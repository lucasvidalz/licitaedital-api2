using LicitaEdital.Core.Identity.MembershipAggregate;
using LicitaEdital.Core.Identity.OrganizationAggregate;
using LicitaEdital.Core.Identity.RoleAggregate;

namespace LicitaEdital.Infrastructure.Data.Identity;

/// <summary>
/// Modulo de identidade e autorizacao: quem entra, em qual organizacao e com quais permissoes.
///
/// **Unico contexto que nao herda de <c>ModuleDbContext</c>**, porque precisa da base do ASP.NET
/// Core Identity. Chama <c>ApplyModuleConventions</c> direto, que e' o mesmo que aquela base faria.
///
/// A base e' <c>IdentityUserContext</c>, e nao <c>IdentityDbContext</c> do framework, de proposito:
/// aquele traria tambem <c>AspNetRoles</c>/<c>AspNetUserRoles</c>, e teriamos **dois** sistemas de
/// papel — o do Identity e o nosso <see cref="Role"/>, que e' quem carrega <c>PermissionCode</c> e
/// area. Dois sistemas de papel divergem na primeira permissao concedida num so deles.
/// </summary>
public class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
  : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserContext<ApplicationUser, Guid>(options),
    IUnitOfWork
{
  internal static readonly string ConfigNamespace = typeof(IdentityDbContext).Namespace + ".Config";

  public DbSet<Organization> Organizations => Set<Organization>();
  public DbSet<Membership> Memberships => Set<Membership>();
  public DbSet<Role> Roles => Set<Role>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(DataSchemaConstants.IdentitySchema);
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyModuleConventions(Assembly.GetExecutingAssembly(), ConfigNamespace);
  }
}
