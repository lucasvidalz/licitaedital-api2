using System.Reflection;
using LicitaEdital.Data;
using LicitaEdital.Data.Engagement;
using LicitaEdital.Domain.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Domain.Engagement.SavedOpportunityAggregate;
using LicitaEdital.Domain.Engagement.SubscriptionAggregate;

namespace LicitaEdital.Queries.Engagement;

/// <summary>
/// Lado de leitura do modulo Engagement: salvas, preferencias de alerta e assinatura.
/// </summary>
public class EngagementReadContext(DbContextOptions<EngagementReadContext> options)
  : ReadOnlyModuleDbContext(options)
{
  public DbSet<SavedOpportunity> SavedOpportunities => Set<SavedOpportunity>();
  public DbSet<AlertPreferences> AlertPreferences => Set<AlertPreferences>();
  public DbSet<Subscription> Subscriptions => Set<Subscription>();

  protected override string Schema => DataSchemaConstants.EngagementSchema;

  protected override string ConfigurationNamespace => typeof(EngagementDbContext).Namespace + ".Config";

  protected override Assembly ConfigurationAssembly => typeof(EngagementDbContext).Assembly;
}
