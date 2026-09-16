using LicitaEdital.Facade.Identity;

namespace LicitaEdital.Application.Identity.ConfirmEmail;

/// <summary>
/// Confirma o e-mail pelo token do link (`AUTH-08`). Nao autentica: quem chega aqui pode estar num
/// navegador diferente do que cadastrou, e confirmar e-mail nao e' prova de posse de senha.
/// </summary>
public class ConfirmEmailHandler(IUserAccountService accounts)
  : ICommandHandler<ConfirmEmailCommand, Result>
{
  private readonly IUserAccountService _accounts = accounts;

  public ValueTask<Result> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    => new(_accounts.ConfirmEmailAsync(command.Token, cancellationToken));
}
