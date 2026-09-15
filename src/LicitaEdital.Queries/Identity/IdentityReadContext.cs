using System.Reflection;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;
using LicitaEdital.Data;
using LicitaEdital.Data.Identity;

namespace LicitaEdital.Queries.Identity;

/// <summary>
/// Lado de leitura do modulo Identity. Sem rastreamento e recusando gravacao, como todo contexto de
/// leitura.
///
/// **Nao herda do `IdentityDbContext`**: aquele traz a base do ASP.NET Core Identity, que registra
/// as tabelas de usuario. Aqui so' precisamos de vinculo e papel — o e-mail e o nome vem do
/// `IUserAccountService`, que e' quem fala com o provedor de identidade.
/// </summary>
public class IdentityReadContext(DbContextOptions<IdentityReadContext> options)
  : ReadOnlyModuleDbContext(options)
{
  public DbSet<Membership> Memberships => Set<Membership>();
  public DbSet<Role> Roles => Set<Role>();

  protected override string Schema => DataSchemaConstants.IdentitySchema;

  protected override string ConfigurationNamespace => typeof(IdentityDbContext).Namespace + ".Config";

  protected override Assembly ConfigurationAssembly => typeof(IdentityDbContext).Assembly;
}
