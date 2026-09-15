using LicitaEdital.Core.Catalog.Facade;
using LicitaEdital.Facade.Catalog;
using Microsoft.Extensions.DependencyInjection;

namespace LicitaEdital.Facade;

public static class FacadeServiceExtensions
{
  /// <summary>
  /// Registra a fachada de cada modulo. Uma fachada nova entra aqui **e** ganha seu contrato em
  /// `Core/&lt;Modulo&gt;/Facade/` — nunca so a implementacao, senao o modulo consumidor teria que
  /// referenciar este projeto para enxergar o tipo, e a dependencia entre modulos voltaria por
  /// outro caminho.
  /// </summary>
  public static IServiceCollection AddModuleFacades(this IServiceCollection services)
  {
    services.AddScoped<ICatalogFacade, CatalogFacade>();

    return services;
  }
}
