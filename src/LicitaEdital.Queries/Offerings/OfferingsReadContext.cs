using System.Reflection;
using LicitaEdital.Data;
using LicitaEdital.Data.Offerings;
using LicitaEdital.Domain.Offerings.OfferingAggregate;

namespace LicitaEdital.Queries.Offerings;

/// <summary>
/// Lado de leitura do modulo Offerings. Sem rastreamento e recusando gravacao, como todo contexto de
/// leitura; o mapeamento vem das configuracoes do <see cref="OfferingsDbContext"/>.
/// </summary>
public class OfferingsReadContext(DbContextOptions<OfferingsReadContext> options)
  : ReadOnlyModuleDbContext(options)
{
  public DbSet<Offering> Offerings => Set<Offering>();

  protected override string Schema => DataSchemaConstants.OfferingsSchema;

  protected override string ConfigurationNamespace => typeof(OfferingsDbContext).Namespace + ".Config";

  protected override Assembly ConfigurationAssembly => typeof(OfferingsDbContext).Assembly;
}
