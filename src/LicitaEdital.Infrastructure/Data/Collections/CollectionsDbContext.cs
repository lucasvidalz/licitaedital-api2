using LicitaEdital.Core.Collections.CollectionRunAggregate;
using LicitaEdital.Core.Collections.CoverageSettingsAggregate;

namespace LicitaEdital.Infrastructure.Data.Collections;

/// <summary>
/// Operacao da coleta: cada execucao do coletor e ate onde o radar cobre. Dado de plataforma, lido
/// pela area de gerenciamento — nao ha tenant em nenhuma das duas tabelas.
/// </summary>
public class CollectionsDbContext(DbContextOptions<CollectionsDbContext> options) : ModuleDbContext(options)
{
  protected override string Schema => DataSchemaConstants.CollectionsSchema;

  protected override string ConfigurationNamespace => typeof(CollectionsDbContext).Namespace + ".Config";

  public DbSet<CollectionRun> CollectionRuns => Set<CollectionRun>();
  public DbSet<CoverageSettings> CoverageSettings => Set<CoverageSettings>();
}
