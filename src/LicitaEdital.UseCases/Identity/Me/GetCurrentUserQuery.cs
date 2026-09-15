namespace LicitaEdital.UseCases.Identity.Me;

public sealed record GetCurrentUserQuery : IQuery<Result<AuthenticatedUserDto>>;
