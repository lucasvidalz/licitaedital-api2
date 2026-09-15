using LicitaEdital.Facade.Identity;

namespace LicitaEdital.Providers.Smtp;

/// <summary>
/// Avisos de conta por e-mail.
///
/// <para>
/// <b>Engole a falha de envio de proposito.</b> Em `forgot-password`, deixar a excecao subir faria
/// o endpoint responder diferente conforme o e-mail existisse — que e' exatamente a fuga que
/// `AUTH-06` fecha. Em `register`, derrubaria um cadastro ja concluido por causa de um e-mail que o
/// usuario leria depois. A falha vai para o log, com o endereco **mascarado**.
/// </para>
/// </summary>
public class EmailAuthenticationNotifier(
  IEmailSender emailSender,
  IOptions<AuthNotificationOptions> options,
  ILogger<EmailAuthenticationNotifier> logger) : IAuthenticationNotifier
{
  private readonly IEmailSender _emailSender = emailSender;
  private readonly AuthNotificationOptions _options = options.Value;
  private readonly ILogger<EmailAuthenticationNotifier> _logger = logger;

  public Task SendPasswordResetAsync(string email, string displayName, string token,
    CancellationToken cancellationToken = default)
    => SendAsync(email, "Redefinição de senha",
      $"""
       Olá, {displayName}.

       Para definir uma nova senha, acesse: {_options.ResetPasswordUrl}?token={Uri.EscapeDataString(token)}

       Se não foi você quem pediu, ignore esta mensagem — sua senha continua a mesma.
       """);

  public Task SendEmailConfirmationAsync(string email, string displayName, string token,
    CancellationToken cancellationToken = default)
    => SendAsync(email, "Confirme seu e-mail",
      $"""
       Olá, {displayName}.

       Para confirmar seu e-mail, acesse: {_options.ConfirmEmailUrl}?token={Uri.EscapeDataString(token)}
       """);

  private async Task SendAsync(string to, string subject, string body)
  {
    try
    {
      await _emailSender.SendEmailAsync(to, _options.FromAddress, subject, body);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Falha ao enviar '{Subject}' para {Recipient}", subject, Mask(to));
    }
  }

  /// <summary>`fulano@exemplo.com` vira `f****@exemplo.com`: o log identifica sem expor.</summary>
  private static string Mask(string email)
  {
    var at = email.IndexOf('@');
    return at <= 1 ? "***" : $"{email[0]}****{email[at..]}";
  }
}

/// <summary>URLs das telas do frontend e remetente. Configuradas em `Auth:Notifications`.</summary>
public class AuthNotificationOptions
{
  public string FromAddress { get; set; } = "nao-responda@licitaedital.com.br";
  public string ResetPasswordUrl { get; set; } = "http://localhost:4200/reset-password";
  public string ConfirmEmailUrl { get; set; } = "http://localhost:4200/confirm-email";
}
