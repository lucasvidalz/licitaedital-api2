using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Application.Identity.Users.Get;

public sealed record GetUserQuery(UserId UserId) : IQuery<Result<UserListItemDto>>;
