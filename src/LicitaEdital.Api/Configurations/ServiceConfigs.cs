using LicitaEdital.BuildingBlocks.Application;
using LicitaEdital.BuildingBlocks.Auth;
using LicitaEdital.Data;
using LicitaEdital.Data.Identity;
using LicitaEdital.Providers;
using LicitaEdital.Queries;
using Microsoft.AspNetCore.Identity;

namespace LicitaEdital.Api.Configurations;

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

    // Politica de credencial (comprimento, composicao, bloqueio) e provedores de token vem da
    // lib: sao padrao da casa, nao escolha deste produto. Aqui ficam so' as tres respostas que
    // so' esta aplicacao tem — qual usuario, qual store, e a excecao deliberada ao padrao.
    services.AddBuildingBlocksIdentity<ApplicationUser>(identity =>
            {
              identity.SignIn.RequireConfirmedEmail = false; // FEAT-12.2: cadastro autentica na hora
            })
            .AddEntityFrameworkStores<IdentityDbContext>();

    services
      .AddDataServices(config, logger) // escrita: contextos de modulo e repositorios
      .AddQueryServices(config)        // leitura: contextos sem rastreamento
      .AddProviders(config)            // integracoes com sistema externo
      .AddMediatorSourceGen(logger);

    logger.LogInformation("{Project} services registered",
      "BuildingBlocks, Data, Queries, Providers e Mediator");

    return services;
  }
}
