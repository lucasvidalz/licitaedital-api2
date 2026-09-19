using System.Reflection;
using LicitaEdital.Data;
using LicitaEdital.Data.Collections;
using LicitaEdital.Domain.Collections.CollectionRunAggregate;
using LicitaEdital.Domain.Collections.CoverageSettingsAggregate;

namespace LicitaEdital.Queries.Collections;

/// <summary>
/// Lado de leitura do modulo Collections: execucoes do coletor e cobertura do radar.
///
/// <b>Sem tenant.</b> Os dois agregados sao da plataforma, nao de um cliente — quem os protege e' a
/// area `manager` no endpoint, nao um filtro de organizacao.
/// </summary>
public class CollectionsReadContext(DbContextOptions<CollectionsReadContext> options)
  : ReadOnlyModuleDbContext(options)
{
  public DbSet<CollectionRun> CollectionRuns => Set<CollectionRun>();
  public DbSet<CoverageSettings> CoverageSettings => Set<CoverageSettings>();

  protected override string Schema => DataSchemaConstants.CollectionsSchema;

  protected override string ConfigurationNamespace => typeof(CollectionsDbContext).Namespace + ".Config";

  protected override Assembly ConfigurationAssembly => typeof(CollectionsDbContext).Assembly;
}
