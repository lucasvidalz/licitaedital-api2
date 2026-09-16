using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Application.Identity.Users.List;

public class ListUsersHandler(IExecutionContext execution, IUsersQueryService users)
  : IQueryHandler<ListUsersQuery, Result<PagedList<UserListItemDto>>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IUsersQueryService _users = users;

  public async ValueTask<Result<PagedList<UserListItemDto>>> Handle(ListUsersQuery query,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId)
    {
      return Result<PagedList<UserListItemDto>>.Unauthorized();
    }

    var filter = new ListUsersFilter(OrganizationId.From(tenantId), query.Search, query.Status);

    return await _users.ListAsync(filter, query.Page, cancellationToken);
  }
}
