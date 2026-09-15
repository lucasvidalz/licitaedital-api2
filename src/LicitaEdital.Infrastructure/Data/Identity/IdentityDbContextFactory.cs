namespace LicitaEdital.Infrastructure.Data.Identity;

/// <summary>Factory de design-time do modulo Identity. Ver <see cref="ModuleDbContextFactory{TContext}"/>.</summary>
public class IdentityDbContextFactory : ModuleDbContextFactory<IdentityDbContext>
{
  protected override string Schema => DataSchemaConstants.IdentitySchema;
}
