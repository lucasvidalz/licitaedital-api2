namespace LicitaEdital.Data.Collections;

/// <summary>Factory de design-time do modulo Collections.</summary>
public class CollectionsDbContextFactory : NpgsqlModuleDbContextFactory<CollectionsDbContext>
{
  protected override string Schema => DataSchemaConstants.CollectionsSchema;
}
