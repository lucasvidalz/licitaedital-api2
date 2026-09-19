namespace LicitaEdital.Api.Opportunities;

/// <summary>
/// As 2 rotas de `/opportunities`. Os valores sao contrato — `opportunities-api.service.ts` e
/// `opportunity-details-api.service.ts` as montam literalmente.
/// </summary>
public static class OpportunitiesRoutes
{
  public const string List = "/opportunities";
  public const string Get = "/opportunities/{id}";
}
