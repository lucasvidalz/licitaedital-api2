namespace LicitaEdital.Queries.Contracts.Identity.Login;

/// <summary>Autentica credenciais. Quem emite o cookie e' o endpoint — sessao e' assunto de HTTP.</summary>
public sealed record LoginCommand(string Email, string Password)
  : ICommand<Result<AuthenticatedUserDto>>;
