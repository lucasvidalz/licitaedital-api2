using LicitaEdital.Domain.Companies.CompanyProfileAggregate;
using LicitaEdital.Queries.Contracts.Companies;

namespace LicitaEdital.Queries.Companies;

public class CompanyProfileQueryService(CompaniesReadContext context) : ICompanyProfileQueryService
{
  private readonly CompaniesReadContext _context = context;

  public async Task<CompanyProfileDto?> FindAsync(OrganizationId organizationId,
    CancellationToken cancellationToken = default)
  {
    // Filtro por organizacao explicito, e nao confiado ao filtro global: e' o unico criterio que
    // separa o cadastro de uma empresa do de outra, e o indice unico
    // `ux_company_profiles_organization` garante que ha no maximo uma linha.
    var profile = await _context.CompanyProfiles
      .FirstOrDefaultAsync(candidate => candidate.OrganizationId == organizationId, cancellationToken);

    return profile is null ? null : ToDto(profile);
  }

  private static CompanyProfileDto ToDto(CompanyProfile profile) => new(
    profile.CompanyName.Value,
    // Sai **normalizado**, sem pontuacao: e' como o dominio guarda, e a mascara e' decisao da tela.
    profile.Cnpj.Value,
    profile.City,
    profile.State.Value,
    profile.BusinessArea);
}
