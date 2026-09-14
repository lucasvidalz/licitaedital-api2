using LicitaEdital.Core.Companies.CompanyProfileAggregate;

namespace LicitaEdital.Infrastructure.Data.Companies;

/// <summary>Cadastro da empresa do cliente. Le e grava `/company-profile`.</summary>
public class CompaniesDbContext(DbContextOptions<CompaniesDbContext> options) : ModuleDbContext(options)
{
  protected override string Schema => DataSchemaConstants.CompaniesSchema;

  protected override string ConfigurationNamespace => typeof(CompaniesDbContext).Namespace + ".Config";

  public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();
}
