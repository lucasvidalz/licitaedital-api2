using LicitaEdital.Core.Collections.CollectionRunAggregate;
using LicitaEdital.Core.Collections.CoverageSettingsAggregate;
using LicitaEdital.Infrastructure.Data.Conventions;

namespace LicitaEdital.Infrastructure.Data.Collections;

/// <summary>
/// Operacao da coleta: cada execucao do coletor e ate onde o radar cobre. Dado de plataforma, lido
/// pela area de gerenciamento — nao ha `OrganizationId` em nenhuma das duas tabelas.
/// </summary>
public class CollectionsDbContext(DbContextOptions<CollectionsDbContext> options) : DbContext(options)
{
  internal static readonly string ConfigNamespace = typeof(CollectionsDbContext).Namespace + ".Config";

  public DbSet<CollectionRun> CollectionRuns => Set<CollectionRun>();
  public DbSet<CoverageSettings> CoverageSettings => Set<CoverageSettings>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(DataSchemaConstants.CollectionsSchema);
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
      type => type.Namespace == ConfigNamespace);
    modelBuilder.UseSnakeCaseNames();
  }
}
