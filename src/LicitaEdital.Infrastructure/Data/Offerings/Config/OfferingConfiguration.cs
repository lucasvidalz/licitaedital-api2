using LicitaEdital.Core.Offerings.OfferingAggregate;
using LicitaEdital.Core.Shared;
using LicitaEdital.Infrastructure.Data.Config;

namespace LicitaEdital.Infrastructure.Data.Offerings.Config;

public class OfferingConfiguration : IEntityTypeConfiguration<Offering>
{
  public void Configure(EntityTypeBuilder<Offering> builder)
  {
    builder.ToTable("offerings");

    builder.Property(offering => offering.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(offering => offering.OrganizationId).HasVogenConversion().IsRequired();

    builder.Property(offering => offering.Name)
      .HasVogenConversion()
      .HasMaxLength(OfferingName.MaxLength)
      .IsRequired();

    builder.Property(offering => offering.Description)
      .HasMaxLength(DataSchemaConstants.DefaultTextLength)
      .IsRequired();

    builder.Property(offering => offering.SupplyType)
      .HasConversion(type => type.Value, value => SmartEnum<SupplyType, string>.FromValue(value))
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    // Os quatro conjuntos de termos em `text[]`: o motor de compatibilidade os le inteiros, e o
    // PostgreSQL ainda permite filtrar por sobreposicao com indice GIN quando o volume pedir.
    builder.PrimitiveCollection<List<string>>("_positiveKeywords")
      .HasColumnName("positive_keywords").HasColumnType("text[]")
      .UsePropertyAccessMode(PropertyAccessMode.Field).IsRequired();

    builder.PrimitiveCollection<List<string>>("_negativeKeywords")
      .HasColumnName("negative_keywords").HasColumnType("text[]")
      .UsePropertyAccessMode(PropertyAccessMode.Field).IsRequired();

    builder.PrimitiveCollection<List<string>>("_synonyms")
      .HasColumnName("synonyms").HasColumnType("text[]")
      .UsePropertyAccessMode(PropertyAccessMode.Field).IsRequired();

    builder.PrimitiveCollection<List<string>>("_catalogCodes")
      .HasColumnName("catalog_codes").HasColumnType("text[]")
      .UsePropertyAccessMode(PropertyAccessMode.Field).IsRequired();

    // UFs atendidas: value object, entao precisa de conversor explicito para `text[]`.
    builder.Property<List<StateCode>>("_servedRegions")
      .HasColumnName("served_regions")
      .HasColumnType("text[]")
      .HasConversion(CollectionConverters.StateCodeList(), CollectionConverters.StateCodeListComparer())
      .UsePropertyAccessMode(PropertyAccessMode.Field)
      .IsRequired();

    builder.Property(offering => offering.MinValueCents);
    builder.Property(offering => offering.MaxValueCents);

    // `GET /offerings` lista tudo da organizacao, sem paginacao.
    builder.HasIndex(offering => offering.OrganizationId)
      .HasDatabaseName("ix_offerings_organization");

    // Nome unico por organizacao: duas ofertas homonimas tornariam ilegivel a origem de um score.
    builder.HasIndex(offering => new { offering.OrganizationId, offering.Name })
      .IsUnique()
      .HasDatabaseName("ux_offerings_organization_name")
      .ActiveOnly();

    builder.UseXminAsConcurrencyToken();
  }
}
