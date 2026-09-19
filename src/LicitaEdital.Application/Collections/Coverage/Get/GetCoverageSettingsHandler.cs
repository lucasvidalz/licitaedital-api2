using LicitaEdital.Queries.Contracts.Collections;

namespace LicitaEdital.Application.Collections.Coverage.Get;

/// <summary>**Nunca responde 404**: sem cobertura gravada, o padrao do dominio.</summary>
public class GetCoverageSettingsHandler(ICoverageSettingsQueryService coverage)
  : IQueryHandler<GetCoverageSettingsQuery, Result<CoverageSettingsDto>>
{
  private readonly ICoverageSettingsQueryService _coverage = coverage;

  public async ValueTask<Result<CoverageSettingsDto>> Handle(GetCoverageSettingsQuery query,
    CancellationToken cancellationToken)
    => await _coverage.GetAsync(cancellationToken);
}
