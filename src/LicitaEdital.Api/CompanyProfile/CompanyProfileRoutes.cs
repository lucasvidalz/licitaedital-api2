namespace LicitaEdital.Api.CompanyProfile;

/// <summary>
/// As 2 rotas de `/company-profile`. Recurso **singular**: sem `{id}`, porque a organizacao ja vem
/// da sessao. Os valores sao contrato — `company-profile-api.service.ts` monta a partir de
/// `PATH = 'company-profile'`.
/// </summary>
public static class CompanyProfileRoutes
{
  public const string Get = "/company-profile";
  public const string Save = "/company-profile";
}
