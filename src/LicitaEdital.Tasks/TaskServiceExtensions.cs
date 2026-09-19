using Microsoft.Extensions.DependencyInjection;

namespace LicitaEdital.Tasks;

public static class TaskServiceExtensions
{
  /// <summary>
  /// Os handlers de evento de dominio sao descobertos pelo **source generator** do Mediator, a
  /// partir da lista de assemblies em `Api/Configurations/MediatorConfig.cs` — nao ha registro
  /// manual aqui. Este metodo existe para o que Tasks vier a ter de proprio: fila, agendamento,
  /// servico hospedado — e para o que **nao** e' handler, como o
  /// <see cref="Catalog.CompatibilityRecalculator"/>, que os dois handlers de compatibilidade
  /// compartilham.
  /// </summary>
  public static IServiceCollection AddTasks(this IServiceCollection services)
    => services.AddScoped<Catalog.CompatibilityRecalculator>();
}
