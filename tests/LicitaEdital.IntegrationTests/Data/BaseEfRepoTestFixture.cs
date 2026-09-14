using LicitaEdital.Infrastructure.Data;

namespace LicitaEdital.IntegrationTests.Data;

/// <summary>
/// Contexto de modulo apontando para um banco InMemory novo a cada fixture.
///
/// **Limite conhecido, e ele importa:** o provedor InMemory nao tem `text[]`, `xmin`, schema nem
/// indice unico. Vale para testar comportamento de agregado e de repositorio; **nao** vale para
/// mapeamento, constraint ou concorrencia — esses vao para os testes funcionais, que usam
/// PostgreSQL em container.
/// </summary>
public abstract class BaseEfRepoTestFixture<TContext> where TContext : DbContext
{
  protected readonly TContext _dbContext;

  protected BaseEfRepoTestFixture()
  {
    _dbContext = CreateContext();
  }

  protected static TContext CreateContext()
  {
    var fakeEventDispatcher = Substitute.For<IDomainEventDispatcher>();

    var serviceProvider = new ServiceCollection()
        .AddEntityFrameworkInMemoryDatabase()
        .AddScoped<IDomainEventDispatcher>(_ => fakeEventDispatcher)
        .AddScoped<EventDispatchInterceptor>()
        .BuildServiceProvider();

    var interceptor = serviceProvider.GetRequiredService<EventDispatchInterceptor>();

    var builder = new DbContextOptionsBuilder<TContext>();
    builder.UseInMemoryDatabase($"licitaedital-{Guid.CreateVersion7()}")
           .UseInternalServiceProvider(serviceProvider)
           .AddInterceptors(interceptor);

    return (TContext)Activator.CreateInstance(typeof(TContext), builder.Options)!;
  }

  protected EfRepository<TContext, T> GetRepository<T>() where T : class, IAggregateRoot
      => new(_dbContext);
}
