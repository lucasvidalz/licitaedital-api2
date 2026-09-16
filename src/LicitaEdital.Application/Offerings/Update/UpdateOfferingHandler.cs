using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Domain.Offerings.OfferingAggregate;
using LicitaEdital.Domain.Offerings.OfferingAggregate.Specifications;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Offerings;

namespace LicitaEdital.Application.Offerings.Update;

public class UpdateOfferingHandler(
  IExecutionContext execution,
  IRepository<Offering> offerings,
  IOfferingsQueryService reader)
  : ICommandHandler<UpdateOfferingCommand, Result<OfferingDto>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IRepository<Offering> _offerings = offerings;
  private readonly IOfferingsQueryService _reader = reader;

  public async ValueTask<Result<OfferingDto>> Handle(UpdateOfferingCommand command,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<OfferingDto>.Unauthorized();

    var organizationId = OrganizationId.From(tenantId);

    // Oferta de outra organizacao cai aqui pela propria especificacao, e sai como 404 — nunca 403
    // (spec §16).
    var offering = await _offerings.FirstOrDefaultAsync(
      new OfferingByIdSpec(organizationId, command.OfferingId), cancellationToken);

    if (offering is null) return Result<OfferingDto>.NotFound();

    var name = command.Payload.Name();

    // Colisao de nome com **outra** oferta. Renomear para o proprio nome continua valendo — e' o
    // caso comum de salvar o formulario sem mexer no titulo.
    var homonym = await _offerings.FirstOrDefaultAsync(new OfferingByNameSpec(organizationId, name),
      cancellationToken);

    if (homonym is not null && homonym.Id != offering.Id)
    {
      return Result<OfferingDto>.Invalid(
        new ValidationError("name", "Ja existe uma oferta com este nome."));
    }

    command.Payload.ApplyTo(offering);

    await _offerings.UpdateAsync(offering, cancellationToken);

    var saved = await _reader.FindAsync(organizationId, offering.Id, cancellationToken);

    return saved is null
      ? throw new InvalidOperationException("Oferta recem-gravada nao pode ser lida de volta.")
      : saved;
  }
}
