using Ardalis.GuardClauses;
using LicitaEdital.Query.Catalog;
using LicitaEdital.UseCases.Catalog.Opportunities.List;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LicitaEdital.Query;

public static class QueryServiceExtensions
{
  /// <summary>
  /// Connection string do lado de leitura. Quando ausente, cai na de escrita — que e' o caso hoje,
  /// com uma base so. O nome separado existe para que apontar uma replica de leitura amanha seja
  /// uma linha de configuracao, e nao uma refatoracao.
  /// </summary>
  public const string ReadConnectionStringName = "licitaedital-read";

  public static IServiceCollection AddQueryServices(this IServiceCollection services,
    IConfiguration config)
  {
    var connectionString = config.GetConnectionString(ReadConnectionStringName)
                           ?? config.GetConnectionString("licitaedital");
    Guard.Against.NullOrWhiteSpace(connectionString, nameof(connectionString));

    // Contexto de leitura por modulo. **Sem os interceptors** de auditoria, soft delete, tenant e
    // eventos: eles so fazem sentido quando ha gravacao, e este contexto recusa gravar.
    // Sem `MigrationsHistoryTable`: este contexto nunca migra. Quem cria e evolui o schema e' o
    // lado de escrita, e ter os dois apontando para a mesma tabela de historico so convidaria
    // alguem a rodar `dotnet ef` contra o contexto errado.
    services.AddDbContext<CatalogReadContext>(options => options.UseNpgsql(connectionString));

    services.AddScoped<IListOpportunitiesQueryService, ListOpportunitiesQueryService>();

    return services;
  }
}
