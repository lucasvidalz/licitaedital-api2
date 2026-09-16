using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Application.Identity.Users.Get;

/// <summary>
/// `GET /users/{id}`. Usuario de outra organizacao responde **404, nao 403** (spec §16): 403 avisa
/// que o recurso existe, e isso ja e' informacao sobre a base de outro cliente.
/// </summary>
public class GetUserHandler(IExecutionContext execution, IUsersQueryService users)
  : IQueryHandler<GetUserQuery, Result<UserListItemDto>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IUsersQueryService _users = users;

  public async ValueTask<Result<UserListItemDto>> Handle(GetUserQuery query,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<UserListItemDto>.Unauthorized();

    var user = await _users.FindAsync(OrganizationId.From(tenantId), query.UserId, cancellationToken);

    return user is null ? Result<UserListItemDto>.NotFound() : user;
  }
}
