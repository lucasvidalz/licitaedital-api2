using LicitaEdital.BuildingBlocks.Application;
using LicitaEdital.BuildingBlocks.Auth;
using LicitaEdital.Facade;
using LicitaEdital.Infrastructure;
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
            .AddBuildingBlocksAuth();

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
