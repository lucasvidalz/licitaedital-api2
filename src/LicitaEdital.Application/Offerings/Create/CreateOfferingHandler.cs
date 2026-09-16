using Ardalis.SmartEnum;
using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Domain.Offerings.OfferingAggregate;
using LicitaEdital.Domain.Offerings.OfferingAggregate.Specifications;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Offerings;

namespace LicitaEdital.Application.Offerings.Create;

public class CreateOfferingHandler(
  IExecutionContext execution,
  IRepository<Offering> offerings,
  IOfferingsQueryService reader)
  : ICommandHandler<CreateOfferingCommand, Result<OfferingDto>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IRepository<Offering> _offerings = offerings;
  private readonly IOfferingsQueryService _reader = reader;

  public async ValueTask<Result<OfferingDto>> Handle(CreateOfferingCommand command,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<OfferingDto>.Unauthorized();

    var organizationId = OrganizationId.From(tenantId);
    var name = command.Payload.Name();

    // Nome unico por organizacao (`ux_offerings_organization_name`). Conferir antes transforma a
    // violacao de indice — excecao, 500 — num erro de campo que o formulario mostra.
    if (await _offerings.FirstOrDefaultAsync(new OfferingByNameSpec(organizationId, name),
          cancellationToken) is not null)
    {
      return Result<OfferingDto>.Invalid(
        new ValidationError("name", "Ja existe uma oferta com este nome."));
    }

    var offering = Offering.Create(organizationId, name, command.Payload.Description,
      SmartEnum<SupplyType, string>.FromValue(command.Payload.SupplyType));

    command.Payload.ApplyTo(offering);

    await _offerings.AddAsync(offering, cancellationToken);

    return await ReadBackAsync(organizationId, offering.Id, cancellationToken);
  }

  private async Task<Result<OfferingDto>> ReadBackAsync(OrganizationId organizationId,
    OfferingId offeringId, CancellationToken cancellationToken)
  {
    // Relê pela consulta da tela: o corpo do 201 e' igual ao que `GET /offerings` devolveria, entao a
    // store do frontend pode inserir a linha sem pedir a lista de novo.
    var saved = await _reader.FindAsync(organizationId, offeringId, cancellationToken);

    return saved is null
      ? throw new InvalidOperationException("Oferta recem-criada nao pode ser lida de volta.")
      : saved;
  }
}
