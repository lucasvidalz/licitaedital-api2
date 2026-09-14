namespace LicitaEdital.Infrastructure.Data;

/// <summary>
/// Um schema PostgreSQL por modulo (D-01). O schema e' a fronteira fisica: nenhuma consulta
/// atravessa, nenhuma FK aponta para fora, e a permissao do banco pode ser concedida por modulo.
/// </summary>
public static class DataSchemaConstants
{
  public const string IdentitySchema = "identity";
  public const string CompaniesSchema = "companies";
  public const string CatalogSchema = "catalog";
  public const string OfferingsSchema = "offerings";
  public const string EngagementSchema = "engagement";
  public const string CollectionsSchema = "collections";

  /// <summary>Tabela de historico de migracao, replicada em cada schema de modulo.</summary>
  public const string MigrationsHistoryTable = "__ef_migrations_history";

  public const int DefaultNameLength = 200;
  public const int DefaultCodeLength = 40;
  public const int DefaultTextLength = 2000;
  public const int UrlLength = 2048;
}
