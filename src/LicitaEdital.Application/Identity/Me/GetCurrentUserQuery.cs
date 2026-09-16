namespace LicitaEdital.Application.Identity.Me;

public sealed record GetCurrentUserQuery : IQuery<Result<AuthenticatedUserDto>>;
