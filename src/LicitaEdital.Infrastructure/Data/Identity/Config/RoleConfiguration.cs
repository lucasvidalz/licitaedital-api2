using LicitaEdital.Core.Identity.MembershipAggregate;
using LicitaEdital.Core.Identity.RoleAggregate;

namespace LicitaEdital.Infrastructure.Data.Identity.Config;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
  public void Configure(EntityTypeBuilder<Role> builder)
  {
    builder.ToTable("roles");

    builder.Property(role => role.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(role => role.Name)
      .HasVogenConversion()
      .HasMaxLength(RoleName.MaxLength)
      .IsRequired();

    builder.Property(role => role.Area)
      .HasConversion(area => area.Value, value => SmartEnum<UserArea, string>.FromValue(value))
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    // As permissoes moram num `text[]` da propria linha, nao numa tabela de juncao: o conjunto e'
    // sempre lido inteiro (para montar `AuthUser.permissions`) e nunca consultado isoladamente.
    // O campo e' privado — o agregado controla conceder e revogar.
    builder.Property<List<PermissionCode>>("_permissions")
      .HasColumnName("permissions")
      .HasColumnType("text[]")
      .HasConversion(
        SmartEnumCollectionConverters.SmartEnumList<PermissionCode>(),
        SmartEnumCollectionConverters.SmartEnumListComparer<PermissionCode>())
      .UsePropertyAccessMode(PropertyAccessMode.Field)
      .IsRequired();

    // `Permissions` e' projecao de leitura sobre o campo `_permissions`, ja mapeado acima como
    // `text[]`. Sem este Ignore, a convencao do EF ve uma colecao de **classe** e tenta mapear
    // `PermissionCode` como entidade propria — com tabela, chave e construtor que ele nao consegue
    // vincular. O SmartEnum nao e' entidade; e' valor.
    builder.Ignore(role => role.Permissions);

    builder.HasIndex(role => role.Name).IsUnique().ActiveOnly();

    builder.UseXminConcurrencyToken();
  }
}
