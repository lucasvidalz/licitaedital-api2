using LicitaEdital.Domain.Offerings.OfferingAggregate;

namespace LicitaEdital.Data.Offerings;

/// <summary>O que cada cliente vende. Entrada do motor de compatibilidade.</summary>
public class OfferingsDbContext(DbContextOptions<OfferingsDbContext> options) : ModuleDbContext(options)
{
  protected override string Schema => DataSchemaConstants.OfferingsSchema;

  protected override string ConfigurationNamespace => typeof(OfferingsDbContext).Namespace + ".Config";

  public DbSet<Offering> Offerings => Set<Offering>();
}
