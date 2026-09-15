namespace LicitaEdital.Infrastructure.Data.Catalog;

/// <summary>Factory de design-time do modulo Catalog. Ver <see cref="ModuleDbContextFactory{TContext}"/>.</summary>
public class CatalogDbContextFactory : ModuleDbContextFactory<CatalogDbContext>
{
  protected override string Schema => DataSchemaConstants.CatalogSchema;
}
