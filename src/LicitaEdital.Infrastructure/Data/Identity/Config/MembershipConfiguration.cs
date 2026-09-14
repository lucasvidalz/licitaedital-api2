using LicitaEdital.Core.Identity.MembershipAggregate;

namespace LicitaEdital.Infrastructure.Data.Identity.Config;

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
  public void Configure(EntityTypeBuilder<Membership> builder)
  {
    builder.ToTable("memberships");

    builder.Property(membership => membership.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(membership => membership.OrganizationId).HasVogenConversion().IsRequired();
    builder.Property(membership => membership.UserId).HasVogenConversion().IsRequired();
    builder.Property(membership => membership.RoleId).HasVogenConversion().IsRequired();

    builder.Property(membership => membership.Area)
      .HasConversion(area => area.Value, value => SmartEnum<UserArea, string>.FromValue(value))
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    builder.Property(membership => membership.Status)
      .HasConversion(status => status.Value, value => SmartEnum<MembershipStatus, string>.FromValue(value))
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    builder.Property(membership => membership.JoinedAt).IsRequired();
    builder.Property(membership => membership.UpdatedAt).IsRequired();
    builder.Property(membership => membership.LastLoginAt);

    // Um vinculo por par (organizacao, usuario). Sem isto, convidar duas vezes daria dois vinculos
    // com papeis diferentes e `GET /auth/me` teria duas respostas possiveis para a mesma sessao.
    builder.HasIndex(membership => new { membership.OrganizationId, membership.UserId })
      .IsUnique()
      .HasDatabaseName("ux_memberships_organization_user");

    // `GET /users?status=&search=` pagina por organizacao e situacao.
    builder.HasIndex(membership => new { membership.OrganizationId, membership.Status })
      .HasDatabaseName("ix_memberships_organization_status");

    builder.UseXminAsConcurrencyToken();
  }
}
