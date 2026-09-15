using LicitaEdital.Core.Identity.Interfaces;

namespace LicitaEdital.UseCases.Identity.ForgotPassword;

/// <summary>
/// Envia o link de redefinicao — e **nunca revela se o e-mail existe** (`FEAT-12.3`, `AUTH-06`).
///
/// <para>
/// Devolver sempre <c>Success</c> resolve metade do problema. A outra metade e' o **tempo**: se o
/// caminho do e-mail existente esperasse o SMTP e o inexistente voltasse na hora, a diferenca
/// apareceria na aba Network e a protecao seria cosmetica. Por isso o envio e' disparo-e-esquece em
/// <see cref="IAuthenticationNotifier"/> — os dois caminhos fazem o mesmo trabalho sincrono: uma
/// busca por e-mail.
/// </para>
///
/// <para>
/// E' o achado 1 do `DF-004` do `licitaledital-api`, que nenhuma regra `SEC-NN` cobre. Resolvido
/// aqui, e nao no texto da tela.
/// </para>
/// </summary>
public class ForgotPasswordHandler(IUserAccountService accounts, IAuthenticationNotifier notifier)
  : ICommandHandler<ForgotPasswordCommand, Result>
{
  private readonly IUserAccountService _accounts = accounts;
  private readonly IAuthenticationNotifier _notifier = notifier;

  public async ValueTask<Result> Handle(ForgotPasswordCommand command,
    CancellationToken cancellationToken)
  {
    var account = await _accounts.FindByEmailAsync(command.Email, cancellationToken);

    if (account is not null)
    {
      var token = await _accounts.GeneratePasswordResetTokenAsync(account.Id, cancellationToken);
      await _notifier.SendPasswordResetAsync(account.Email, account.DisplayName, token,
        cancellationToken);
    }

    // Mesmo resultado nos dois caminhos. Nao logue qual deles ocorreu: log tambem e' canal de
    // vazamento, e quem le o log nem sempre e' quem deveria.
    return Result.Success();
  }
}
