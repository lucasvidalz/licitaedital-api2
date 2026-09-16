using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Domain.Companies.CompanyProfileAggregate;
using LicitaEdital.Domain.Companies.CompanyProfileAggregate.Specifications;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Companies;

namespace LicitaEdital.Application.Companies.Profile.Save;

/// <summary>
/// Cria ou substitui o cadastro da empresa da organizacao da sessao.
/// </summary>
public class SaveCompanyProfileHandler(
  IExecutionContext execution,
  IRepository<CompanyProfile> profiles,
  ICompanyProfileQueryService reader)
  : ICommandHandler<SaveCompanyProfileCommand, Result<CompanyProfileDto>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IRepository<CompanyProfile> _profiles = profiles;
  private readonly ICompanyProfileQueryService _reader = reader;

  public async ValueTask<Result<CompanyProfileDto>> Handle(SaveCompanyProfileCommand command,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<CompanyProfileDto>.Unauthorized();

    var organizationId = OrganizationId.From(tenantId);

    var cnpj = Cnpj.Parse(command.Cnpj);
    var state = StateCode.Parse(command.State);
    var companyName = CompanyName.From(command.CompanyName);

    // O CNPJ e' unico na plataforma, nao so' na organizacao (`ux_company_profiles_cnpj`). Conferir
    // antes transforma o que seria uma violacao de indice — excecao, 500 — num erro de campo que o
    // formulario sabe mostrar. A corrida continua coberta pelo indice: dois cadastros simultaneos do
    // mesmo CNPJ ainda esbarram nele, e ai' o 500 e' a resposta honesta para um caso raro.
    var owner = await _profiles.FirstOrDefaultAsync(new CompanyProfileByCnpjSpec(cnpj),
      cancellationToken);

    if (owner is not null && owner.OrganizationId != organizationId)
    {
      // A mensagem nao confirma **qual** organizacao usa o CNPJ, nem que ela existe: quem cadastra
      // so' precisa saber que aquele numero nao esta disponivel para ele.
      return Result<CompanyProfileDto>.Invalid(
        new ValidationError("cnpj", "Este CNPJ ja esta cadastrado em outra conta."));
    }

    var profile = await _profiles.FirstOrDefaultAsync(
      new CompanyProfileByOrganizationSpec(organizationId), cancellationToken);

    if (profile is null)
    {
      profile = CompanyProfile.Create(organizationId, companyName, cnpj, command.City, state,
        command.BusinessArea);
      await _profiles.AddAsync(profile, cancellationToken);
    }
    else
    {
      profile.Update(companyName, cnpj, command.City, state, command.BusinessArea);
      await _profiles.UpdateAsync(profile, cancellationToken);
    }

    // Relê pela consulta da tela: e' a mesma projecao de `GET /company-profile`, entao o corpo da
    // resposta do `PUT` e' identico ao que um reload devolveria.
    var saved = await _reader.FindAsync(organizationId, cancellationToken);

    return saved is null
      ? throw new InvalidOperationException("Perfil recem-gravado nao pode ser lido de volta.")
      : saved;
  }
}
