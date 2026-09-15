namespace LicitaEdital.Data.Identity;

/// <summary>Factory de design-time do modulo Identity.</summary>
public class IdentityDbContextFactory : NpgsqlModuleDbContextFactory<IdentityDbContext>
{
  protected override string Schema => DataSchemaConstants.IdentitySchema;
}
