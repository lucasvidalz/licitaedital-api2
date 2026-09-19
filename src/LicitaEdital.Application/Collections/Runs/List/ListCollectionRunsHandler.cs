using LicitaEdital.Queries.Contracts.Collections;

namespace LicitaEdital.Application.Collections.Runs.List;

public class ListCollectionRunsHandler(ICollectionRunsQueryService runs)
  : IQueryHandler<ListCollectionRunsQuery, Result<CollectionRunsDto>>
{
  private readonly ICollectionRunsQueryService _runs = runs;

  public async ValueTask<Result<CollectionRunsDto>> Handle(ListCollectionRunsQuery query,
    CancellationToken cancellationToken)
    => await _runs.ListAsync(query.Page, cancellationToken);
}
