namespace LicitaEdital.Queries.Contracts.Identity.Me;

public sealed record GetCurrentUserQuery : IQuery<Result<AuthenticatedUserDto>>;
