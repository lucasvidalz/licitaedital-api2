using LicitaEdital.BuildingBlocks.Application;
using LicitaEdital.BuildingBlocks.Auth;
using LicitaEdital.Core.Interfaces;
using LicitaEdital.Infrastructure;
using LicitaEdital.Infrastructure.Email;

namespace LicitaEdital.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services,
    Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
  {
    // Base proprietaria: TimeProvider, despachante de eventos e contexto de execucao (Application);
    // sessao por cookie, XSRF e policies de permissao/area (Auth).
    services.AddBuildingBlocksApplication()
            .AddBuildingBlocksAuth();

    services.AddInfrastructureServices(builder.Configuration, logger)
            .AddMediatorSourceGen(logger);

    // O servidor de e-mail local e' configurado pelo Aspire (Papercut).
    // Ver: https://ardalis.com/configuring-a-local-test-email-server/
    services.AddScoped<IEmailSender, MimeKitEmailSender>();

    logger.LogInformation("{Project} services registered", "BuildingBlocks, Infrastructure, Mediator e Email");

    return services;
  }
}
