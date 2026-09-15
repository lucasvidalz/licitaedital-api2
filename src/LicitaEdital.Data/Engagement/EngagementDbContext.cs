using LicitaEdital.Domain.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Domain.Engagement.SavedOpportunityAggregate;
using LicitaEdital.Domain.Engagement.SubscriptionAggregate;

namespace LicitaEdital.Data.Engagement;

/// <summary>Como cada cliente se relaciona com o radar: o que salvou, o que quer receber e em que plano esta.</summary>
public class EngagementDbContext(DbContextOptions<EngagementDbContext> options) : ModuleDbContext(options)
{
  protected override string Schema => DataSchemaConstants.EngagementSchema;

  protected override string ConfigurationNamespace => typeof(EngagementDbContext).Namespace + ".Config";

  public DbSet<SavedOpportunity> SavedOpportunities => Set<SavedOpportunity>();
  public DbSet<AlertPreferences> AlertPreferences => Set<AlertPreferences>();
  public DbSet<Subscription> Subscriptions => Set<Subscription>();
}
