namespace LicitaEdital.Queries.Contracts.Companies;

/// <summary>
/// Leitura do perfil da empresa.
///
/// Devolve <c>null</c> quando a organizacao ainda nao cadastrou — e' o **primeiro estado normal** do
/// ciclo de vida, nao uma falha. O endpoint traduz para 404 e a tela abre formulario vazio
/// (`AD-032`); nao logue como erro.
/// </summary>
public interface ICompanyProfileQueryService
{
  Task<CompanyProfileDto?> FindAsync(OrganizationId organizationId,
    CancellationToken cancellationToken = default);
}
