namespace LicitaEdital.Data.Identity.Config;

/// <summary>
/// Renomeia as tabelas do ASP.NET Core Identity para o padrao do projeto. Sem isto conviveriam
/// <c>aspnetusers</c> e <c>memberships</c> na mesma base — duas convencoes de nome no mesmo schema.
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
  public void Configure(EntityTypeBuilder<ApplicationUser> builder)
  {
    builder.ToTable("users");

    builder.Property(user => user.DisplayName)
      .HasMaxLength(ApplicationUser.DisplayNameMaxLength)
      .IsRequired();

    // Proprio do ApplicationUser: ele nao herda a auditoria da lib, porque a base dele e
    // o IdentityUser do framework.
    builder.Property(user => user.CreatedAt).IsRequired();

    // Unicidade de e-mail e' do Identity (NormalizedEmail), mas ela chega como indice nao-unico
    // por default: um mesmo e-mail poderia registrar duas contas. `POST /auth/register` precisa
    // falhar no banco, nao so na validacao da aplicacao.
    builder.HasIndex(user => user.NormalizedEmail)
      .IsUnique()
      .HasDatabaseName("ix_users_normalized_email");
  }
}
