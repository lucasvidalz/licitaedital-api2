using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Facade.Identity;

/// <summary>
/// Avisos de conta. Separado de <c>IEmailSender</c> de proposito: o caso de uso pede "avise que a
/// senha pode ser redefinida", nao "monte um HTML e mande por SMTP". Quem decide canal, template e
/// idioma e' a implementacao.
///
/// <para>
/// <b>Todo metodo aqui e' disparo-e-esquece do ponto de vista do caso de uso.</b> Em
/// `forgot-password` isso nao e' conveniencia, e' requisito: se o envio acontecesse em linha, o
/// tempo de resposta denunciaria se o e-mail existe — a mesma fuga que a mensagem generica tenta
/// fechar (`AUTH-06`, achado 1 do `DF-004`).
/// </para>
/// </summary>
public interface IAuthenticationNotifier
{
  Task SendPasswordResetAsync(string email, string displayName, string token,
    CancellationToken cancellationToken = default);

  Task SendEmailConfirmationAsync(string email, string displayName, string token,
    CancellationToken cancellationToken = default);
}
