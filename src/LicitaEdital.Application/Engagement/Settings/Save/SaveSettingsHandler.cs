using Ardalis.SmartEnum;
using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Domain.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Domain.Engagement.AlertPreferencesAggregate.Specifications;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Engagement;

namespace LicitaEdital.Application.Engagement.Settings.Save;

public class SaveSettingsHandler(
  IExecutionContext execution,
  IRepository<AlertPreferences> preferences,
  IAlertPreferencesQueryService reader)
  : ICommandHandler<SaveSettingsCommand, Result<SettingsDto>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IRepository<AlertPreferences> _preferences = preferences;
  private readonly IAlertPreferencesQueryService _reader = reader;

  public async ValueTask<Result<SettingsDto>> Handle(SaveSettingsCommand command,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<SettingsDto>.Unauthorized();

    var organizationId = OrganizationId.From(tenantId);

    var types = new AlertTypes(command.NewCompatibleOpportunity, command.ApproachingDeadline,
      command.DailySummary);
    var frequency = SmartEnum<AlertFrequency, string>.FromValue(command.Frequency);
    var filter = new AlertFilter(
      [.. command.States.Select(StateCode.Parse)],
      [.. command.Modalities],
      SmartEnum<ValueRange, string>.FromValue(command.ValueRange));

    var existing = await _preferences.FirstOrDefaultAsync(
      new AlertPreferencesByOrganizationSpec(organizationId), cancellationToken);

    if (existing is null)
    {
      // Nasce do padrao e so' entao recebe o corpo: o construtor do agregado e' privado de proposito,
      // e `CreateDefault` e' a unica porta de entrada — o que garante que campo novo acrescentado ao
      // agregado tenha um valor sensato mesmo quando o cliente nao o envia.
      existing = AlertPreferences.CreateDefault(organizationId);
      existing.Update(types, frequency, filter);
      await _preferences.AddAsync(existing, cancellationToken);
    }
    else
    {
      existing.Update(types, frequency, filter);
      await _preferences.UpdateAsync(existing, cancellationToken);
    }

    return await _reader.GetAsync(organizationId, cancellationToken);
  }
}
