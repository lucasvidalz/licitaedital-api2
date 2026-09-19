using LicitaEdital.Queries.Contracts.Collections;

namespace LicitaEdital.Api.Gfe;

/// <summary>`CollectionRunApiItem` do contrato.</summary>
public sealed record CollectionRunResponse(
  Guid Id,
  DateTimeOffset StartedAt,
  DateTimeOffset EndedAt,
  int NewOpportunitiesCount,
  string Result,
  string? ErrorMessage);

/// <summary>
/// `CollectionRunsResponse` do contrato: paginacao **mais** `lastSuccessfulRunAt`.
///
/// Envelope proprio, e nao o `PagedResponse&lt;T&gt;` padrao — o ultimo sucesso nao pertence a
/// pagina, e poe-lo no envelope generico o tornaria um campo opcional que toda outra listagem
/// carregaria sem usar.
/// </summary>
public sealed record CollectionRunsResponse(
  IReadOnlyList<CollectionRunResponse> Items,
  int Page,
  int PageSize,
  int TotalItems,
  int TotalPages,
  DateTimeOffset? LastSuccessfulRunAt)
{
  public static CollectionRunsResponse From(CollectionRunsDto runs) => new(
    [.. runs.Items.Select(run => new CollectionRunResponse(run.Id, run.StartedAt, run.EndedAt,
      run.NewOpportunitiesCount, run.Result, run.ErrorMessage))],
    runs.Page, runs.PageSize, runs.TotalItems, runs.TotalPages, runs.LastSuccessfulRunAt);
}

/// <summary>`CoverageSettingsApiItem` do contrato.</summary>
public sealed record CoverageSettingsResponse(
  IReadOnlyList<string> AttendedStates,
  string ValueRange)
{
  public static CoverageSettingsResponse From(CoverageSettingsDto coverage)
    => new(coverage.AttendedStates, coverage.ValueRange);
}
