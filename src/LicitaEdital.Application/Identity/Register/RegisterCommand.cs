namespace LicitaEdital.Application.Identity.Register;

public sealed record RegisterCommand(string Email, string Password, string DisplayName)
  : ICommand<Result<AuthenticatedUserDto>>;
