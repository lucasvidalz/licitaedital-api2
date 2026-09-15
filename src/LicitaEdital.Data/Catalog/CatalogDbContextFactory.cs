namespace LicitaEdital.Data.Catalog;

/// <summary>Factory de design-time do modulo Catalog.</summary>
public class CatalogDbContextFactory : NpgsqlModuleDbContextFactory<CatalogDbContext>
{
  protected override string Schema => DataSchemaConstants.CatalogSchema;
}
