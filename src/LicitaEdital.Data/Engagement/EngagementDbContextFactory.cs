namespace LicitaEdital.Data.Engagement;

/// <summary>Factory de design-time do modulo Engagement.</summary>
public class EngagementDbContextFactory : NpgsqlModuleDbContextFactory<EngagementDbContext>
{
  protected override string Schema => DataSchemaConstants.EngagementSchema;
}
