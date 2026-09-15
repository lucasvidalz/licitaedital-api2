namespace LicitaEdital.Queries.Contracts.Identity.Register;

public sealed record RegisterCommand(string Email, string Password, string DisplayName)
  : ICommand<Result<AuthenticatedUserDto>>;
