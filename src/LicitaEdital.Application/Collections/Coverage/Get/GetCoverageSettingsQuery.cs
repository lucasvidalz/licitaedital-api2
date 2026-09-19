using LicitaEdital.Queries.Contracts.Collections;

namespace LicitaEdital.Application.Collections.Coverage.Get;

public sealed record GetCoverageSettingsQuery : IQuery<Result<CoverageSettingsDto>>;
