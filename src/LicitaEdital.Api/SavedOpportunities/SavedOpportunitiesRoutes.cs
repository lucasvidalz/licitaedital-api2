namespace LicitaEdital.Api.SavedOpportunities;

/// <summary>
/// As 3 rotas de `/saved-opportunities`.
///
/// O `{opportunityId}` do `DELETE` e' o id da **licitacao**, nao o do registro de salvamento — e' o
/// unico id que a tela tem em maos, e e' o que o contrato define.
/// </summary>
public static class SavedOpportunitiesRoutes
{
  public const string List = "/saved-opportunities";
  public const string Save = "/saved-opportunities";
  public const string Unsave = "/saved-opportunities/{opportunityId}";
}
