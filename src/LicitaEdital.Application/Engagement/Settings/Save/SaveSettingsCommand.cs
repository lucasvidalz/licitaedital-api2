using LicitaEdital.Queries.Contracts.Engagement;

namespace LicitaEdital.Application.Engagement.Settings.Save;

/// <summary>
/// `PUT /settings` — substitui as preferencias inteiras. Os campos chegam como string porque e'
/// assim que o contrato os traz; virar SmartEnum e' trabalho da fronteira, onde entrada invalida
/// ainda vira 400 com o campo nomeado.
/// </summary>
public sealed record SaveSettingsCommand(
  bool NewCompatibleOpportunity,
  bool ApproachingDeadline,
  bool DailySummary,
  string Frequency,
  IReadOnlyList<string> States,
  IReadOnlyList<string> Modalities,
  string ValueRange) : ICommand<Result<SettingsDto>>;
