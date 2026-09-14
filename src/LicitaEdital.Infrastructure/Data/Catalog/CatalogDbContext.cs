using LicitaEdital.Core.Catalog.CompatibilityAggregate;
using LicitaEdital.Core.Catalog.OpportunityAggregate;
using LicitaEdital.Infrastructure.Data.Conventions;

namespace LicitaEdital.Infrastructure.Data.Catalog;

/// <summary>
/// Licitacoes publicadas e a compatibilidade calculada para cada organizacao.
///
/// A compatibilidade vive **aqui**, e nao em Offerings, para que
/// `GET /opportunities?sort=score` filtre, ordene e pagine numa consulta so. Com a nota em outro
/// schema, a listagem principal do produto exigiria join entre modulos — proibido por D-01 — ou
/// duas consultas que nao paginam juntas.
/// </summary>
public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
  internal static readonly string ConfigNamespace = typeof(CatalogDbContext).Namespace + ".Config";

  public DbSet<Opportunity> Opportunities => Set<Opportunity>();
  public DbSet<OpportunityCompatibility> Compatibilities => Set<OpportunityCompatibility>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(DataSchemaConstants.CatalogSchema);
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
      type => type.Namespace == ConfigNamespace);
    modelBuilder.UseSnakeCaseNames();
  }
}
