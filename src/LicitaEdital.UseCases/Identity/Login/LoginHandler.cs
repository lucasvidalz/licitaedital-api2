using LicitaEdital.Core.Identity.Interfaces;

namespace LicitaEdital.UseCases.Identity.Login;

public class LoginHandler(
  IUserAccountService accounts,
  IAuthenticatedUserReader reader)
  : ICommandHandler<LoginCommand, Result<AuthenticatedUserDto>>
{
  private readonly IUserAccountService _accounts = accounts;
  private readonly IAuthenticatedUserReader _reader = reader;

  public async ValueTask<Result<AuthenticatedUserDto>> Handle(LoginCommand command,
    CancellationToken cancellationToken)
  {
    var account = await _accounts.FindByEmailAsync(command.Email, cancellationToken);

    // **Uma unica resposta para todos os caminhos de falha**: e-mail inexistente, senha errada,
    // conta bloqueada e vinculo suspenso saem iguais. Diferenciar transformaria a tela de login num
    // verificador de quais e-mails existem na base.
    if (account is null) return InvalidCredentials();

    if (!await _accounts.CheckPasswordAsync(account.Id, command.Password, cancellationToken))
    {
      return InvalidCredentials();
    }

    var user = await _reader.ReadAsync(account.Id, cancellationToken);
    if (user is null) return InvalidCredentials();

    await _accounts.RegisterSuccessfulLoginAsync(account.Id, cancellationToken);

    return user;
  }

  private static Result<AuthenticatedUserDto> InvalidCredentials()
    => Result<AuthenticatedUserDto>.Unauthorized();
}
