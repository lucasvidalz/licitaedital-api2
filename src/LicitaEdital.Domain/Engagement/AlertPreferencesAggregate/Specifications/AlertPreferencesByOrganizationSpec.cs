using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Engagement.AlertPreferencesAggregate.Specifications;

/// <summary>As preferencias de uma organizacao. Singular — `PUT /settings` e' upsert.</summary>
public sealed class AlertPreferencesByOrganizationSpec : SingleResultSpecification<AlertPreferences>
{
  public AlertPreferencesByOrganizationSpec(OrganizationId organizationId)
  {
    Query.Where(preferences => preferences.OrganizationId == organizationId);
  }
}
