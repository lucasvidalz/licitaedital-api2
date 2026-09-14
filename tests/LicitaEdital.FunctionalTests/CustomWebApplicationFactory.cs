using LicitaEdital.Infrastructure;
using LicitaEdital.Infrastructure.Data.Catalog;
using LicitaEdital.Infrastructure.Data.Collections;
using LicitaEdital.Infrastructure.Data.Companies;
using LicitaEdital.Infrastructure.Data.Engagement;
using LicitaEdital.Infrastructure.Data.Identity;
using LicitaEdital.Infrastructure.Data.Offerings;
using Testcontainers.PostgreSql;

namespace LicitaEdital.FunctionalTests;

/// <summary>
/// Sobe a API contra um PostgreSQL real em container.
///
/// **Nao ha fallback para banco em memoria.** O template tinha um, para SQLite, e ele escondia
/// exatamente o que este projeto precisa verificar: `text[]`, `xmin` como token de concorrencia,
/// schema por modulo e indice unico composto nao existem em provedor InMemory — um teste que passa
/// sem Docker nao prova nada sobre a base que vai para producao. Sem Docker, o teste falha.
/// </summary>
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
  where TProgram : class
{
  private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
    .WithImage("postgres:17-alpine")
    .WithDatabase("licitaedital_test")
    .Build();

  public async ValueTask InitializeAsync() => await _dbContainer.StartAsync();

  public new async ValueTask DisposeAsync() => await _dbContainer.DisposeAsync();

  protected override IHost CreateHost(IHostBuilder builder)
  {
    builder.UseEnvironment("Testing"); // nao envia e-mail de verdade
    var host = builder.Build();
    host.Start();

    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

    try
    {
      // Migracao, e nao EnsureCreated: o teste funcional precisa exercitar o mesmo caminho que o
      // deploy. Um schema criado por atalho passaria com migracao quebrada.
      services.GetRequiredService<IdentityDbContext>().Database.Migrate();
      services.GetRequiredService<CompaniesDbContext>().Database.Migrate();
      services.GetRequiredService<CatalogDbContext>().Database.Migrate();
      services.GetRequiredService<OfferingsDbContext>().Database.Migrate();
      services.GetRequiredService<EngagementDbContext>().Database.Migrate();
      services.GetRequiredService<CollectionsDbContext>().Database.Migrate();

      // Semente especifica de cada teste entra no proprio teste ou numa fixture dele — nunca aqui.
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred preparing the test database. Error: {exceptionMessage}", ex.Message);
      throw;
    }

    return host;
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
    => builder.ConfigureAppConfiguration((_, config) =>
      config.AddInMemoryCollection(new Dictionary<string, string?>
      {
        [$"ConnectionStrings:{InfrastructureServiceExtensions.ConnectionStringName}"] =
          _dbContainer.GetConnectionString()
      }));
}
