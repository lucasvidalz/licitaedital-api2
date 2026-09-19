using LicitaEdital.Queries.Contracts.Collections;

namespace LicitaEdital.Application.Collections.Coverage.Save;

public sealed record SaveCoverageSettingsCommand(
  IReadOnlyList<string> AttendedStates,
  string ValueRange) : ICommand<Result<CoverageSettingsDto>>;
