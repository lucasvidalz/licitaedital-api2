using System.Reflection;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.OrganizationAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;
using LicitaEdital.Data;
using LicitaEdital.Data.Identity;

namespace LicitaEdital.Queries.Identity;

/// <summary>
/// Lado de leitura do modulo Identity. Sem rastreamento e recusando gravacao, como todo contexto de
/// leitura.
///
/// **Nao herda do `IdentityDbContext`**: aquele traz a base do ASP.NET Core Identity, com claims,
/// logins e tokens — tabelas que nenhuma tela le. O que chega aqui e' so' o que a
/// `ApplicationUserConfiguration` descreve, porque este contexto aplica as **mesmas**
/// configuracoes do lado de escrita (ver <see cref="ConfigurationAssembly"/>).
/// </summary>
public class IdentityReadContext(DbContextOptions<IdentityReadContext> options)
  : ReadOnlyModuleDbContext(options)
{
  public DbSet<Membership> Memberships => Set<Membership>();
  public DbSet<Role> Roles => Set<Role>();
  public DbSet<Organization> Organizations => Set<Organization>();

  /// <summary>
  /// A tela de usuarios, ja com vinculo, conta e organizacao juntos. Ver
  /// <see cref="UserDirectoryRow"/> para o porque de ser SQL e nao LINQ.
  /// </summary>
  public DbSet<UserDirectoryRow> UserDirectory => Set<UserDirectoryRow>();

  protected override string Schema => DataSchemaConstants.IdentitySchema;

  protected override string ConfigurationNamespace => typeof(IdentityDbContext).Namespace + ".Config";

  protected override Assembly ConfigurationAssembly => typeof(IdentityDbContext).Assembly;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // `HasNoKey`: e' uma projecao, nao uma tabela — o EF nao deve rastrear identidade nem tentar
    // gravar. Os `HasColumnName` sao explicitos para nao depender da convencao de nomes: aqui o
    // nome vem do apelido no SQL, e as duas fontes precisam concordar exatamente.
    modelBuilder.Entity<UserDirectoryRow>(row =>
    {
      row.HasNoKey();
      row.ToSqlQuery(UserDirectorySql);
      row.Property(directory => directory.UserId).HasColumnName("user_id");
      row.Property(directory => directory.OrganizationId).HasColumnName("organization_id");
      row.Property(directory => directory.Email).HasColumnName("email");
      row.Property(directory => directory.DisplayName).HasColumnName("display_name");
      row.Property(directory => directory.Status).HasColumnName("status");
      row.Property(directory => directory.CreatedAt).HasColumnName("created_at");
      row.Property(directory => directory.LastLoginAt).HasColumnName("last_login_at");
      row.Property(directory => directory.CompanyName).HasColumnName("company_name");
    });
  }

  /// <summary>
  /// O SQL de <see cref="UserDirectoryRow"/>. Sem <c>WHERE</c> de organizacao de proposito: o
  /// isolamento entre clientes e' aplicado por quem consulta (<c>UsersQueryService</c>), onde ele
  /// fica visivel e testavel — escondido aqui, ninguem lembraria de procura-lo.
  ///
  /// <c>is_active</c> nas duas pontas repete o que o filtro global faria nas entidades com chave.
  /// </summary>
  private const string UserDirectorySql = """
    SELECT m.user_id         AS user_id,
           m.organization_id AS organization_id,
           u.email           AS email,
           u.display_name    AS display_name,
           m.status          AS status,
           u.created_at      AS created_at,
           m.last_login_at   AS last_login_at,
           o.name            AS company_name
      FROM identity.memberships m
      JOIN identity.users u         ON u.id = m.user_id
      JOIN identity.organizations o ON o.id = m.organization_id
     WHERE m.is_active AND o.is_active
    """;
}
