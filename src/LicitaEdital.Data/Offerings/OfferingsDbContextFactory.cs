namespace LicitaEdital.Data.Offerings;

/// <summary>Factory de design-time do modulo Offerings.</summary>
public class OfferingsDbContextFactory : NpgsqlModuleDbContextFactory<OfferingsDbContext>
{
  protected override string Schema => DataSchemaConstants.OfferingsSchema;
}
