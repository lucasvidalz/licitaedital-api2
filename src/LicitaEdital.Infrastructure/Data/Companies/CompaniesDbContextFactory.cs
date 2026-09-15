namespace LicitaEdital.Infrastructure.Data.Companies;

/// <summary>Factory de design-time do modulo Companies.</summary>
public class CompaniesDbContextFactory : NpgsqlModuleDbContextFactory<CompaniesDbContext>
{
  protected override string Schema => DataSchemaConstants.CompaniesSchema;
}
