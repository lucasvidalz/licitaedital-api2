using Ardalis.SharedKernel;
using LicitaEdital.Core.Interfaces;
using LicitaEdital.Infrastructure;
using LicitaEdital.UseCases;

namespace LicitaEdital.Web.Configurations;

public static class MediatorConfig
{
  // Chamado por ServiceConfigs.cs, nunca por Program.cs
  public static IServiceCollection AddMediatorSourceGen(this IServiceCollection services,
    Microsoft.Extensions.Logging.ILogger logger)
  {
    logger.LogInformation("Registering Mediator SourceGen and Behaviors");
    services.AddMediator(options =>
    {
      // Singleton e o mais rapido segundo a doc; Scoped/Transient tambem sao suportados.
      options.ServiceLifetime = ServiceLifetime.Scoped;

      // Um TYPE qualquer de cada assembly a ser varrido (o gerador descobre o assembly pelo tipo).
      options.Assemblies =
      [
        typeof(IEmailSender),                    // Core
        typeof(Constants),                      // UseCases
        typeof(InfrastructureServiceExtensions), // Infrastructure
        typeof(MediatorConfig)                  // Web
      ];

      // Pipeline behaviors (a ordem importa)
      options.PipelineBehaviors =
      [
        typeof(LoggingBehavior<,>)
      ];
    });

    return services;
  }
}
