using LicitaEdital.Infrastructure.Data;

namespace LicitaEdital.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    // Ordem de prioridade das connection strings:
    // 1. "licitaedital" - fornecida pelo Aspire via .WithReference(licitaEditalDb)
    // 2. "DefaultConnection" - SQL Server (so no Windows por padrao, ou com USE_SQL_SERVER=true)
    // 3. "SqliteConnection" - fallback SQLite
    bool isWindows = OperatingSystem.IsWindows();
    bool forceSqlServer = Environment.GetEnvironmentVariable("USE_SQL_SERVER") == "true";

    string? connectionString = config.GetConnectionString("licitaedital")
                               ?? ((isWindows || forceSqlServer) ? config.GetConnectionString("DefaultConnection") : null)
                               ?? config.GetConnectionString("SqliteConnection");
    Guard.Against.Null(connectionString);

    services.AddScoped<EventDispatchInterceptor>();
    services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

    services.AddDbContext<AppDbContext>((provider, options) =>
    {
      var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();

      if (config.GetConnectionString("licitaedital") != null ||
          ((isWindows || forceSqlServer) && config.GetConnectionString("DefaultConnection") != null))
      {
        options.UseSqlServer(connectionString);
      }
      else
      {
        options.UseSqlite(connectionString);
      }

      options.AddInterceptors(eventDispatchInterceptor);
    });

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
           .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

    // Query services (IListXQueryService) e servicos de dominio implementados aqui
    // sao registrados neste ponto.

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
