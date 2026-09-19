namespace LicitaEdital.Queries.Contracts.Collections;

/// <summary>
/// Espelha `CoverageSettingsApiItem` (`private/gfe/settings/models/coverage-settings-api.model.ts`).
///
/// <c>AttendedStates</c> vazio significa **todas as UFs**, nao nenhuma — e' cobertura, nao
/// restricao. <c>ValueRange</c> e' chave simbolica, como em `/settings`.
/// </summary>
public sealed record CoverageSettingsDto(
  IReadOnlyList<string> AttendedStates,
  string ValueRange);
