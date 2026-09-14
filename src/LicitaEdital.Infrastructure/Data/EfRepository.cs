namespace LicitaEdital.Infrastructure.Data;

/// <summary>
/// Repositorio generico sobre <c>Ardalis.Specification</c>, parametrizado tambem pelo contexto.
///
/// O segundo parametro existe por causa de D-01: com um <c>DbContext</c> por modulo nao ha um
/// "contexto default" que o DI possa injetar, entao cada agregado e' registrado fechado, apontando
/// para o contexto do seu modulo. E' o que impede um agregado de Catalog ser lido pelo contexto de
/// Engagement — o erro que a separacao de modulos existe para evitar.
/// </summary>
public class EfRepository<TContext, T>(TContext dbContext)
  : RepositoryBase<T>(dbContext), IReadRepository<T>, IRepository<T>
  where TContext : DbContext
  where T : class, IAggregateRoot
{
}
