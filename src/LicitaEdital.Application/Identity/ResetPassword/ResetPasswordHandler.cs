using LicitaEdital.Facade.Identity;

namespace LicitaEdital.Application.Identity.ResetPassword;

/// <summary>
/// Redefine a senha a partir do token do link (`AUTH-07`).
///
/// Expiracao e uso unico sao do provedor de identidade: trocar a senha muda o `SecurityStamp`, o
/// que invalida o token consumido **e** qualquer sessao aberta. E' o segundo item do `DF-004`
/// resolvido por mecanismo, nao por controle nosso.
/// </summary>
public class ResetPasswordHandler(IUserAccountService accounts)
  : ICommandHandler<ResetPasswordCommand, Result>
{
  private readonly IUserAccountService _accounts = accounts;

  public ValueTask<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    => new(_accounts.ResetPasswordAsync(command.Token, command.Password, cancellationToken));
}
