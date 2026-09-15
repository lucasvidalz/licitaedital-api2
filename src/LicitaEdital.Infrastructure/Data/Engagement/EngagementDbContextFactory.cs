namespace LicitaEdital.Infrastructure.Data.Engagement;

/// <summary>Factory de design-time do modulo Engagement. Ver <see cref="ModuleDbContextFactory{TContext}"/>.</summary>
public class EngagementDbContextFactory : ModuleDbContextFactory<EngagementDbContext>
{
  protected override string Schema => DataSchemaConstants.EngagementSchema;
}
