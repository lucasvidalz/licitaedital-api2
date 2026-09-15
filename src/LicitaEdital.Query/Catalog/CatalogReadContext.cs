using System.Reflection;
using LicitaEdital.Core.Catalog.CompatibilityAggregate;
using LicitaEdital.Core.Catalog.OpportunityAggregate;
using LicitaEdital.Infrastructure.Data;
using LicitaEdital.Infrastructure.Data.Catalog;

namespace LicitaEdital.Query.Catalog;

/// <summary>
/// Lado de leitura do modulo Catalog.
///
/// Herda de <c>ReadOnlyModuleDbContext</c>, entao ja vem sem rastreamento, sem deteccao automatica
/// de alteracao, sem lazy loading e recusando <c>SaveChanges</c> — **nenhuma consulta deste projeto
/// escreve `AsNoTracking()`**, porque o rastreamento nunca chega a ser ligado.
///
/// O mapeamento vem das configuracoes do <see cref="CatalogDbContext"/>, no assembly de
/// persistencia: um mapeamento so para os dois lados.
/// </summary>
public class CatalogReadContext(DbContextOptions<CatalogReadContext> options)
  : ReadOnlyModuleDbContext(options)
{
  public DbSet<Opportunity> Opportunities => Set<Opportunity>();
  public DbSet<OpportunityCompatibility> Compatibilities => Set<OpportunityCompatibility>();

  protected override string Schema => DataSchemaConstants.CatalogSchema;

  protected override string ConfigurationNamespace => typeof(CatalogDbContext).Namespace + ".Config";

  protected override Assembly ConfigurationAssembly => typeof(CatalogDbContext).Assembly;
}
