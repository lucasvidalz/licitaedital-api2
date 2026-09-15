using LicitaEdital.BuildingBlocks.Application.Behaviors;
using LicitaEdital.Facade;
using LicitaEdital.Data;
using LicitaEdital.Application;

namespace LicitaEdital.Api.Configurations;

public static class MediatorConfig
{
  // Chamado por ServiceConfigs.cs, nunca por Program.cs
  public static IServiceCollection AddMediatorSourceGen(this IServiceCollection services,
    Microsoft.Extensions.Logging.ILogger logger)
  {
    logger.LogInformation("Registering Mediator SourceGen and Behaviors");
    services.AddMediator(options =>
    {
      options.ServiceLifetime = ServiceLifetime.Scoped;

      // Um TYPE qualquer de cada assembly a ser varrido (o gerador descobre o assembly pelo tipo).
      options.Assemblies =
      [
        typeof(IEmailSender),                    // Core
        typeof(ApplicationAssembly),               // UseCases
        typeof(DataServiceExtensions), // Infrastructure
        typeof(LicitaEdital.Tasks.TaskServiceExtensions), // Tasks
        typeof(MediatorConfig)                  // Api
      ];

      // Behaviors vem da lib. **A ordem importa**: validar antes de logar o fim da operacao, e
      // ambos antes do handler. Behavior registrado por DI em vez de aqui fica silenciosamente
      // fora do pipeline gerado pelo source generator.
      options.PipelineBehaviors =
      [
        typeof(LoggingBehavior<,>),
        typeof(ValidationBehavior<,>)
      ];
    });

    return services;
  }
}
