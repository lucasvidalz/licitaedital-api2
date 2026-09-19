using LicitaEdital.Queries.Contracts.Engagement;

namespace LicitaEdital.Api.Settings;

/// <summary>`SettingsApiItem` do contrato (`private/cfe/settings/models/settings-api.model.ts`).</summary>
public sealed record SettingsResponse(
  AlertTypesResponse AlertTypes,
  string Frequency,
  SettingsFilterResponse Filter)
{
  public static SettingsResponse From(SettingsDto settings) => new(
    new AlertTypesResponse(settings.AlertTypes.NewCompatibleOpportunity,
      settings.AlertTypes.ApproachingDeadline, settings.AlertTypes.DailySummary),
    settings.Frequency,
    new SettingsFilterResponse(settings.Filter.States, settings.Filter.Modalities,
      settings.Filter.ValueRange));
}

public sealed record AlertTypesResponse(
  bool NewCompatibleOpportunity,
  bool ApproachingDeadline,
  bool DailySummary);

/// <summary>
/// <c>ValueRange</c> viaja como **chave simbolica** (<c>up-to-100k</c>…, e <c>""</c> para "sem
/// faixa"), nunca como centavos: a conversao e' do cliente, e mandar o intervalo daqui congelaria a
/// tabela de faixas no dado gravado.
/// </summary>
public sealed record SettingsFilterResponse(
  IReadOnlyList<string> States,
  IReadOnlyList<string> Modalities,
  string ValueRange);
