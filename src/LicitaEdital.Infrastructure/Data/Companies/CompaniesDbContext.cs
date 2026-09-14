using LicitaEdital.Core.Companies.CompanyProfileAggregate;
using LicitaEdital.Infrastructure.Data.Conventions;

namespace LicitaEdital.Infrastructure.Data.Companies;

/// <summary>Cadastro da empresa do cliente. Le e grava `/company-profile`.</summary>
public class CompaniesDbContext(DbContextOptions<CompaniesDbContext> options) : DbContext(options)
{
  internal static readonly string ConfigNamespace = typeof(CompaniesDbContext).Namespace + ".Config";

  public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(DataSchemaConstants.CompaniesSchema);
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
      type => type.Namespace == ConfigNamespace);
    modelBuilder.UseSnakeCaseNames();
  }
}
