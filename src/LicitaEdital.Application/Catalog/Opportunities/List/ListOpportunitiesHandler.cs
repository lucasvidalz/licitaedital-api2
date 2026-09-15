using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Queries.Contracts.Catalog;

/// <summary>
/// O handler existe mesmo a consulta sendo um repasse, e a razao e' uma so: **e' aqui que o tenant
/// entra**, vindo da sessao (<see cref="IExecutionContext"/>) e nunca do request. Um endpoint que
/// chamasse o query service direto teria que receber a organizacao de algum lugar, e o lugar mais
/// facil seria o payload — que e' exatamente o que SEC-08 proibe.
/// </summary>
public class ListOpportunitiesHandler(
  IListOpportunitiesQueryService queryService,
  IExecutionContext execution)
  : IQueryHandler<ListOpportunitiesQuery, Result<PagedList<OpportunityListItemDto>>>
{
  private readonly IListOpportunitiesQueryService _queryService = queryService;
  private readonly IExecutionContext _execution = execution;

  public async ValueTask<Result<PagedList<OpportunityListItemDto>>> Handle(
    ListOpportunitiesQuery query, CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId)
    {
      return Result<PagedList<OpportunityListItemDto>>.Unauthorized();
    }

    var page = await _queryService.ListAsync(
      OrganizationId.From(tenantId), query.Filter, query.Sort, query.Page, cancellationToken);

    return page;
  }
}
