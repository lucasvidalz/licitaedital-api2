namespace LicitaEdital.Api.Offerings;

/// <summary>
/// As 3 rotas de `/offerings`. Os valores sao contrato — `offerings-api.service.ts` monta a partir
/// de `PATH = 'offerings'`.
/// </summary>
public static class OfferingsRoutes
{
  public const string List = "/offerings";
  public const string Create = "/offerings";
  public const string Update = "/offerings/{id}";
}
