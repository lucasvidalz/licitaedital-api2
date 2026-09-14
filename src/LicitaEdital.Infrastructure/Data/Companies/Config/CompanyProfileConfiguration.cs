using LicitaEdital.Core.Companies.CompanyProfileAggregate;
using LicitaEdital.Core.Shared;

namespace LicitaEdital.Infrastructure.Data.Companies.Config;

public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
  public void Configure(EntityTypeBuilder<CompanyProfile> builder)
  {
    builder.ToTable("company_profiles");

    builder.Property(profile => profile.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(profile => profile.OrganizationId).HasVogenConversion().IsRequired();

    builder.Property(profile => profile.CompanyName)
      .HasVogenConversion()
      .HasMaxLength(CompanyName.MaxLength)
      .IsRequired();

    builder.Property(profile => profile.Cnpj)
      .HasVogenConversion()
      .HasMaxLength(Cnpj.Length)
      .IsFixedLength()
      .IsRequired();

    builder.Property(profile => profile.City)
      .HasMaxLength(DataSchemaConstants.DefaultNameLength)
      .IsRequired();

    builder.Property(profile => profile.State)
      .HasVogenConversion()
      .HasMaxLength(StateCode.Length)
      .IsFixedLength()
      .IsRequired();

    builder.Property(profile => profile.BusinessArea)
      .HasMaxLength(DataSchemaConstants.DefaultNameLength)
      .IsRequired();

    builder.Property(profile => profile.CreatedAt).IsRequired();
    builder.Property(profile => profile.UpdatedAt).IsRequired();

    // Um perfil por organizacao: `GET /company-profile` nao recebe id, entao duas linhas tornariam
    // a resposta arbitraria.
    builder.HasIndex(profile => profile.OrganizationId)
      .IsUnique()
      .HasDatabaseName("ux_company_profiles_organization");

    // CNPJ unico na plataforma inteira: a mesma empresa nao se cadastra em duas organizacoes.
    builder.HasIndex(profile => profile.Cnpj)
      .IsUnique()
      .HasDatabaseName("ux_company_profiles_cnpj");

    builder.UseXminAsConcurrencyToken();
  }
}
