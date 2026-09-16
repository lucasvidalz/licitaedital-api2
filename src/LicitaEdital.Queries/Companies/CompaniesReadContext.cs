using System.Reflection;
using LicitaEdital.Data;
using LicitaEdital.Data.Companies;
using LicitaEdital.Domain.Companies.CompanyProfileAggregate;

namespace LicitaEdital.Queries.Companies;

/// <summary>
/// Lado de leitura do modulo Companies. Sem rastreamento e recusando gravacao, como todo contexto de
/// leitura; o mapeamento vem das configuracoes do <see cref="CompaniesDbContext"/>.
/// </summary>
public class CompaniesReadContext(DbContextOptions<CompaniesReadContext> options)
  : ReadOnlyModuleDbContext(options)
{
  public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();

  protected override string Schema => DataSchemaConstants.CompaniesSchema;

  protected override string ConfigurationNamespace => typeof(CompaniesDbContext).Namespace + ".Config";

  protected override Assembly ConfigurationAssembly => typeof(CompaniesDbContext).Assembly;
}
