using LicitaEdital.Core.Identity.OrganizationAggregate;

namespace LicitaEdital.Infrastructure.Data.Identity.Config;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
  public void Configure(EntityTypeBuilder<Organization> builder)
  {
    builder.ToTable("organizations");

    // Guid v7 nasce no dominio (D-04), nao no banco: `ValueGeneratedNever` impede que o EF tente
    // ler de volta um valor gerado e trate o id definido como "temporario".
    builder.Property(organization => organization.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(organization => organization.Name)
      .HasVogenConversion()
      .HasMaxLength(OrganizationName.MaxLength)
      .IsRequired();

    builder.Property(organization => organization.IsPlatform).IsRequired();

    builder.UseXminAsConcurrencyToken();
  }
}
