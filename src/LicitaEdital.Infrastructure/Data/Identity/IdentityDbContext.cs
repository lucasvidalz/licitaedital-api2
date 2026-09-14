using LicitaEdital.Core.Identity.MembershipAggregate;
using LicitaEdital.Core.Identity.OrganizationAggregate;
using LicitaEdital.Core.Identity.RoleAggregate;
using LicitaEdital.Infrastructure.Data.Conventions;

namespace LicitaEdital.Infrastructure.Data.Identity;

/// <summary>
/// Modulo de identidade e autorizacao: quem entra, em qual organizacao e com quais permissoes.
///
/// A base e' <c>IdentityUserContext</c>, e nao <c>IdentityDbContext</c> do ASP.NET Core, de
/// proposito: aquele traz tambem <c>AspNetRoles</c>/<c>AspNetUserRoles</c>, e teriamos **dois**
/// sistemas de papel — o do Identity e o nosso <see cref="Role"/>, que e' o que carrega
/// <c>PermissionCode</c> e area. Dois sistemas de papel divergem na primeira permissao concedida
/// num so deles.
/// </summary>
public class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
  : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserContext<ApplicationUser, Guid>(options)
{
  internal static readonly string ConfigNamespace = typeof(IdentityDbContext).Namespace + ".Config";

  public DbSet<Organization> Organizations => Set<Organization>();
  public DbSet<Membership> Memberships => Set<Membership>();
  public DbSet<Role> Roles => Set<Role>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(DataSchemaConstants.IdentitySchema);
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
      type => type.Namespace == ConfigNamespace);
    modelBuilder.UseSnakeCaseNames();
  }
}
