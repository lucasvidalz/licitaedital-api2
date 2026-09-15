namespace LicitaEdital.Infrastructure.Data.Collections;

/// <summary>Factory de design-time do modulo Collections. Ver <see cref="ModuleDbContextFactory{TContext}"/>.</summary>
public class CollectionsDbContextFactory : ModuleDbContextFactory<CollectionsDbContext>
{
  protected override string Schema => DataSchemaConstants.CollectionsSchema;
}
