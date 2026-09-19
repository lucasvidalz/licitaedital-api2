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

      // **`options.Assemblies` fica vazio de proposito.** Vazio, o gerador varre todos os assemblies
      // alcancaveis por referencia — que e' o comportamento que este produto quer.
      //
      // Aqui havia uma lista explicita (Facade, Application, Data, Tasks, Api) que **omitia
      // `LicitaEdital.Domain`**. Os eventos de dominio sao declarados la', entao o gerador nunca os
      // registrou como notificacao: o `switch` gerado em `Publish` nascia vazio, e **todo evento de
      // dominio publicado desde sempre caia no vazio — sem erro, sem aviso, sem handler**. Comandos e
      // consultas funcionavam porque moram em `Application`, que estava na lista.
      //
      // A lista nao comprava nada (os assemblies varridos sao os mesmos que ja referenciamos) e
      // custava exatamente este tipo de falha: um projeto novo esquecido ali quebra em silencio, e o
      // sintoma aparece meses depois, como funcionalidade que simplesmente nao acontece.

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
