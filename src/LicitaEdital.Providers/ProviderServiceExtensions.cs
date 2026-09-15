using LicitaEdital.Core.Identity.Interfaces;
using LicitaEdital.Providers.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LicitaEdital.Providers;

public static class ProviderServiceExtensions
{
  /// <summary>
  /// Registra as integracoes com sistema externo. Um diretorio por sistema, um registro aqui.
  ///
  /// Provider que fale HTTP usa <c>AddExternalHttpClient</c> de
  /// <c>LicitaEdital.BuildingBlocks.Integration</c> — e' o que traz a guarda de destino (SSRF),
  /// o bloqueio de redirect automatico, a resiliencia e o timeout. Nunca <c>AddHttpClient</c> nu.
  /// </summary>
  public static IServiceCollection AddProviders(this IServiceCollection services,
    IConfiguration config)
  {
    services.Configure<MailserverConfiguration>(config.GetSection("Mailserver"));
    services.Configure<AuthNotificationOptions>(config.GetSection("Auth:Notifications"));

    services.AddScoped<IEmailSender, MimeKitEmailSender>();
    services.AddScoped<IAuthenticationNotifier, EmailAuthenticationNotifier>();

    return services;
  }
}
