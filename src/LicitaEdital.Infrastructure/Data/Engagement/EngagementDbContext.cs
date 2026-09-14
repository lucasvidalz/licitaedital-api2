using LicitaEdital.Core.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Core.Engagement.SavedOpportunityAggregate;
using LicitaEdital.Core.Engagement.SubscriptionAggregate;

namespace LicitaEdital.Infrastructure.Data.Engagement;

/// <summary>Como cada cliente se relaciona com o radar: o que salvou, o que quer receber e em que plano esta.</summary>
public class EngagementDbContext(DbContextOptions<EngagementDbContext> options) : ModuleDbContext(options)
{
  protected override string Schema => DataSchemaConstants.EngagementSchema;

  protected override string ConfigurationNamespace => typeof(EngagementDbContext).Namespace + ".Config";

  public DbSet<SavedOpportunity> SavedOpportunities => Set<SavedOpportunity>();
  public DbSet<AlertPreferences> AlertPreferences => Set<AlertPreferences>();
  public DbSet<Subscription> Subscriptions => Set<Subscription>();
}
