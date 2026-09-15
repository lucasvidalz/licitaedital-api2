using LicitaEdital.BuildingBlocks.Application;
using LicitaEdital.BuildingBlocks.Auth;
using LicitaEdital.Facade;
using LicitaEdital.Infrastructure;
using LicitaEdital.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using LicitaEdital.Providers;
using LicitaEdital.Query;

namespace LicitaEdital.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services,
    Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
  {
    var config = builder.Configuration;

    // Base proprietaria: TimeProvider, despachante de eventos e contexto de execucao (Application);
    // sessao por cookie, XSRF e policies de permissao/area (Auth).
    services.AddBuildingBlocksApplication()
            // `allowInsecureCookies` em desenvolvimento: o dev roda em http://localhost:8080, e
            // cookie `Secure` nao volta sobre http — o login responderia 200 e a proxima requisicao
            // chegaria sem sessao. Em homolog e producao o cookie e' `Secure` (SEC-02, SEC-47).
            .AddBuildingBlocksAuth(allowInsecureCookies: builder.Environment.IsDevelopment());

    // ASP.NET Core Identity **sem** a camada de papeis dele: quem carrega permissao e area e' o
    // nosso `Role`, e dois sistemas de papel divergem na primeira permissao concedida num so.
    services.AddIdentityCore<ApplicationUser>(identity =>
    {
      identity.User.RequireUniqueEmail = true;

      // Comprimento acima do default (6) e sem exigir simbolo: exigencia de simbolo empurra o
      // usuario para a senha curta com `!` no fim; comprimento maior rende mais entropia real.
      identity.Password.RequiredLength = 8;
      identity.Password.RequireNonAlphanumeric = false;
      identity.Password.RequireUppercase = false;
      identity.Password.RequiredUniqueChars = 4;

      // Bloqueio por tentativa protege a **conta**; o rate limit por IP protege o **servidor**.
      // Os dois sao necessarios e nenhum substitui o outro.
      identity.Lockout.MaxFailedAccessAttempts = 5;
      identity.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
      identity.Lockout.AllowedForNewUsers = true;

      identity.SignIn.RequireConfirmedEmail = false; // FEAT-12.2: cadastro autentica na hora
    })
    .AddEntityFrameworkStores<IdentityDbContext>()
    // Provedores de token de redefinicao e confirmacao, com expiracao e uso unico. E' o `DF-004`
    // do licitaledital-api resolvido por mecanismo do framework, nao por codigo nosso.
    .AddDefaultTokenProviders();

    services
      .AddInfrastructureServices(config, logger)  // escrita: contextos de modulo e repositorios
      .AddQueryServices(config)                   // leitura: contextos sem rastreamento
      .AddModuleFacades()                         // contratos de leitura entre modulos
      .AddProviders(config)                       // integracoes com sistema externo
      .AddMediatorSourceGen(logger);

    logger.LogInformation("{Project} services registered",
      "BuildingBlocks, Infrastructure, Query, Facade, Providers e Mediator");

    return services;
  }
}
