using LicitaEdital.Core.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Core.Engagement.SavedOpportunityAggregate;
using LicitaEdital.Core.Engagement.SubscriptionAggregate;
using LicitaEdital.Infrastructure.Data.Conventions;

namespace LicitaEdital.Infrastructure.Data.Engagement;

/// <summary>
/// Como cada cliente se relaciona com o radar: o que salvou, o que quer receber e em que plano esta.
/// </summary>
public class EngagementDbContext(DbContextOptions<EngagementDbContext> options) : DbContext(options)
{
  internal static readonly string ConfigNamespace = typeof(EngagementDbContext).Namespace + ".Config";

  public DbSet<SavedOpportunity> SavedOpportunities => Set<SavedOpportunity>();
  public DbSet<AlertPreferences> AlertPreferences => Set<AlertPreferences>();
  public DbSet<Subscription> Subscriptions => Set<Subscription>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(DataSchemaConstants.EngagementSchema);
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
      type => type.Namespace == ConfigNamespace);
    modelBuilder.UseSnakeCaseNames();
  }
}
