using LicitaEdital.Core.Offerings.OfferingAggregate;
using LicitaEdital.Infrastructure.Data.Conventions;

namespace LicitaEdital.Infrastructure.Data.Offerings;

/// <summary>O que cada cliente vende. Entrada do motor de compatibilidade.</summary>
public class OfferingsDbContext(DbContextOptions<OfferingsDbContext> options) : DbContext(options)
{
  internal static readonly string ConfigNamespace = typeof(OfferingsDbContext).Namespace + ".Config";

  public DbSet<Offering> Offerings => Set<Offering>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(DataSchemaConstants.OfferingsSchema);
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
      type => type.Namespace == ConfigNamespace);
    modelBuilder.UseSnakeCaseNames();
  }
}
