using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Facade.Catalog;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Engagement;

namespace LicitaEdital.Application.Engagement.SavedOpportunities.List;

public class ListSavedOpportunitiesHandler(
  IExecutionContext execution,
  ISavedOpportunitiesQueryService saved,
  ICatalogFacade catalog)
  : IQueryHandler<ListSavedOpportunitiesQuery, Result<IReadOnlyList<SavedOpportunityView>>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly ISavedOpportunitiesQueryService _saved = saved;
  private readonly ICatalogFacade _catalog = catalog;

  public async ValueTask<Result<IReadOnlyList<SavedOpportunityView>>> Handle(
    ListSavedOpportunitiesQuery query, CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId)
    {
      return Result<IReadOnlyList<SavedOpportunityView>>.Unauthorized();
    }

    var organizationId = OrganizationId.From(tenantId);
    var rows = await _saved.ListAsync(organizationId, cancellationToken);

    return Result<IReadOnlyList<SavedOpportunityView>>.Success(
      await SavedOpportunityAssembler.AssembleAsync(_catalog, organizationId, rows, cancellationToken));
  }
}
