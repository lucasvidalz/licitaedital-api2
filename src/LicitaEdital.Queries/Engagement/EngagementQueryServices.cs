using LicitaEdital.Domain.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Queries.Contracts.Engagement;

namespace LicitaEdital.Queries.Engagement;

public class SavedOpportunitiesQueryService(EngagementReadContext context)
  : ISavedOpportunitiesQueryService
{
  private readonly EngagementReadContext _context = context;

  public async Task<IReadOnlyList<SavedOpportunityRefDto>> ListAsync(OrganizationId organizationId,
    CancellationToken cancellationToken = default)
  {
    // `CreatedAt` da auditoria **e'** o `savedAt` do contrato — nao ha campo proprio para a mesma
    // data, e ter dois seria ter duas verdades.
    var rows = await _context.SavedOpportunities
      .Where(saved => saved.OrganizationId == organizationId)
      .OrderByDescending(saved => saved.CreatedAt)
      .Select(saved => new { saved.OpportunityId, saved.CreatedAt })
      .ToListAsync(cancellationToken);

    return [.. rows.Select(row => new SavedOpportunityRefDto(row.OpportunityId.Value, row.CreatedAt))];
  }

  public Task<bool> ExistsAsync(OrganizationId organizationId, OpportunityId opportunityId,
    CancellationToken cancellationToken = default)
    => _context.SavedOpportunities.AnyAsync(
      saved => saved.OrganizationId == organizationId && saved.OpportunityId == opportunityId,
      cancellationToken);
}

public class AlertPreferencesQueryService(EngagementReadContext context)
  : IAlertPreferencesQueryService
{
  private readonly EngagementReadContext _context = context;

  public async Task<SettingsDto> GetAsync(OrganizationId organizationId,
    CancellationToken cancellationToken = default)
  {
    var preferences = await _context.AlertPreferences
      .FirstOrDefaultAsync(candidate => candidate.OrganizationId == organizationId, cancellationToken);

    // Sem linha gravada, devolve o **padrao do dominio** em vez de nulo. O padrao vive no agregado
    // (`CreateDefault`), nao aqui: se o valor default mudasse em dois lugares, a tela mostraria uma
    // coisa antes de salvar e outra depois.
    return ToDto(preferences ?? AlertPreferences.CreateDefault(organizationId));
  }

  internal static SettingsDto ToDto(AlertPreferences preferences) => new(
    new AlertTypesDto(
      preferences.Types.NewCompatibleOpportunity,
      preferences.Types.ApproachingDeadline,
      preferences.Types.DailySummary),
    preferences.Frequency.Value,
    new SettingsFilterDto(
      [.. preferences.Filter.States.Select(state => state.Value)],
      [.. preferences.Filter.Modalities],
      preferences.Filter.ValueRange.Value));
}

public class SubscriptionQueryService(EngagementReadContext context) : ISubscriptionQueryService
{
  private readonly EngagementReadContext _context = context;

  public async Task<string?> GetPlanIdAsync(OrganizationId organizationId,
    CancellationToken cancellationToken = default)
  {
    var subscription = await _context.Subscriptions
      .FirstOrDefaultAsync(candidate => candidate.OrganizationId == organizationId, cancellationToken);

    return subscription?.PlanId.Value;
  }
}
