using LicitaEdital.BuildingBlocks.Domain.Entities;
using LicitaEdital.BuildingBlocks.Domain.Events;
using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.BuildingBlocks.Persistence;
using LicitaEdital.BuildingBlocks.Persistence.Interceptors;

namespace LicitaEdital.IntegrationTests.Data;

/// <summary>
/// Contexto de modulo apontando para um banco InMemory novo a cada fixture, com os interceptors da
/// lib ligados — e' o que faz auditoria e soft delete valerem tambem no teste.
///
/// **Limite conhecido, e ele importa:** o provedor InMemory nao tem `text[]`, `xmin`, schema, indice
/// unico nem indice parcial. Vale para testar comportamento de agregado e de repositorio; **nao**
/// vale para mapeamento, constraint ou concorrencia — esses vao para os testes funcionais, que usam
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
    var clock = TimeProvider.System;
    var execution = new SystemExecutionContext();
    var dispatcher = Substitute.For<IDomainEventDispatcher>();

    var builder = new DbContextOptionsBuilder<TContext>();
    builder.UseInMemoryDatabase($"licitaedital-{Guid.CreateVersion7()}")
           .AddInterceptors(
             new SoftDeleteInterceptor(clock, execution),
             new AuditInterceptor(clock, execution),
             new TenantGuardInterceptor(execution),
             new DomainEventDispatchInterceptor(dispatcher));

    return (TContext)Activator.CreateInstance(typeof(TContext), builder.Options)!;
  }

  protected EfRepository<TContext, T> GetRepository<T>() where T : class, IAggregateRoot
      => new(_dbContext);
}
