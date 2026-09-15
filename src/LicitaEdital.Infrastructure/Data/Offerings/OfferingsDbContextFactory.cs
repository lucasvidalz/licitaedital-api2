namespace LicitaEdital.Infrastructure.Data.Offerings;

/// <summary>Factory de design-time do modulo Offerings. Ver <see cref="ModuleDbContextFactory{TContext}"/>.</summary>
public class OfferingsDbContextFactory : ModuleDbContextFactory<OfferingsDbContext>
{
  protected override string Schema => DataSchemaConstants.OfferingsSchema;
}
