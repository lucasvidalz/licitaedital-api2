namespace LicitaEdital.Api.Gfe;

/// <summary>
/// As 3 rotas da area de gerenciamento que nao sao de usuario.
///
/// O prefixo `gfe/` esta no **caminho**, e nao so' na pasta, porque o contrato o traz assim
/// (`collections-api.service.ts`, `coverage-settings-api.service.ts`): a area faz parte da URL, o
/// que torna obvio no log e no proxy qual metade do produto foi chamada.
/// </summary>
public static class GfeRoutes
{
  public const string Collections = "/gfe/collections";
  public const string Settings = "/gfe/settings";
}
