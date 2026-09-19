namespace LicitaEdital.Queries.Contracts.Engagement;

/// <summary>
/// Espelha `SettingsApiItem` (`private/cfe/settings/models/settings-api.model.ts`).
///
/// <para>
/// <c>ValueRange</c> e' a **chave simbolica** (<c>up-to-100k</c>…), nunca centavos: a conversao e' do
/// cliente (`build-opportunities-query.helper.ts`), e persistir o intervalo congelaria a tabela de
/// faixas dentro do dado historico — mudar "ate 100k" para "ate 150k" amanha reescreveria o passado.
/// </para>
/// </summary>
public sealed record SettingsDto(
  AlertTypesDto AlertTypes,
  string Frequency,
  SettingsFilterDto Filter);

public sealed record AlertTypesDto(
  bool NewCompatibleOpportunity,
  bool ApproachingDeadline,
  bool DailySummary);

public sealed record SettingsFilterDto(
  IReadOnlyList<string> States,
  IReadOnlyList<string> Modalities,
  string ValueRange);
