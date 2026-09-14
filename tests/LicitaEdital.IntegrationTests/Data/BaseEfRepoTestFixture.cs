using LicitaEdital.Infrastructure.Data;

namespace LicitaEdital.IntegrationTests.Data;

public abstract class BaseEfRepoTestFixture
{
  protected AppDbContext _dbContext;

  protected BaseEfRepoTestFixture()
  {
    var options = CreateNewContextOptions();
    _dbContext = new AppDbContext(options);
  }

  protected static DbContextOptions<AppDbContext> CreateNewContextOptions()
  {
    var fakeEventDispatcher = Substitute.For<IDomainEventDispatcher>();
    // Um service provider novo a cada fixture, e portanto um banco InMemory novo.
    var serviceProvider = new ServiceCollection()
        .AddEntityFrameworkInMemoryDatabase()
        .AddScoped<IDomainEventDispatcher>(_ => fakeEventDispatcher)
        .AddScoped<EventDispatchInterceptor>()
        .BuildServiceProvider();

    var interceptor = serviceProvider.GetRequiredService<EventDispatchInterceptor>();

    var builder = new DbContextOptionsBuilder<AppDbContext>();
    builder.UseInMemoryDatabase("licitaedital")
           .UseInternalServiceProvider(serviceProvider)
           .AddInterceptors(interceptor);

    return builder.Options;
  }

  // Cada teste de repositorio expoe o seu: protected EfRepository<Company> GetRepository() => new(_dbContext);
}
