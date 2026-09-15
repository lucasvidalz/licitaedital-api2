namespace LicitaEdital.Infrastructure.Data.Companies;

/// <summary>Factory de design-time do modulo Companies. Ver <see cref="ModuleDbContextFactory{TContext}"/>.</summary>
public class CompaniesDbContextFactory : ModuleDbContextFactory<CompaniesDbContext>
{
  protected override string Schema => DataSchemaConstants.CompaniesSchema;
}
