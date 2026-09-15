using LicitaEdital.Facade;
using LicitaEdital.Facade.Catalog;
using LicitaEdital.Facade.Identity;
using LicitaEdital.Providers.Catalog;
using LicitaEdital.Providers.Identity;
using LicitaEdital.Providers.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LicitaEdital.Providers;

public static class ProviderServiceExtensions
{
  /// <summary>
  /// Liga cada contrato do <c>Facade</c> a sua implementacao. **Contrato novo em Facade sem linha
  /// aqui falha em runtime, nao na compilacao** — e' o preco de inverter a dependencia, e o motivo
  /// de todas as ligacoes ficarem num metodo so, onde da' para conferir de relance.
  ///
  /// <para>
  /// Nem toda implementacao fala com sistema externo: <c>UserAccountService</c> roda sobre o store
  /// do ASP.NET Identity e <c>CatalogFacade</c> sobre o contexto de leitura de Catalog — os dois sao
  /// servidos pelo **nosso** banco. Quem fala com fora sao os de <c>Smtp/</c> e, adiante,
  /// <c>Pncp/</c> e <c>Storage/</c>; esses usam <c>AddExternalHttpClient</c> da lib, nunca
  /// <c>AddHttpClient</c> nu.
  /// </para>
  /// </summary>
  public static IServiceCollection AddProviders(this IServiceCollection services,
    IConfiguration config)
  {
    services.Configure<MailserverConfiguration>(config.GetSection("Mailserver"));
    services.Configure<AuthNotificationOptions>(config.GetSection("Auth:Notifications"));

    // --- servidos pelo nosso banco ---
    services.AddScoped<IUserAccountService, UserAccountService>();
    services.AddScoped<ICatalogFacade, CatalogFacade>();

    // --- sistema externo ---
    services.AddScoped<IEmailSender, MimeKitEmailSender>();
    services.AddScoped<IAuthenticationNotifier, EmailAuthenticationNotifier>();

    return services;
  }
}
