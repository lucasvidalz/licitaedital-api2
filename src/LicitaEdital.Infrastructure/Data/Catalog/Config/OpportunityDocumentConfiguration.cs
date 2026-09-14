using LicitaEdital.Core.Catalog.OpportunityAggregate;

namespace LicitaEdital.Infrastructure.Data.Catalog.Config;

public class OpportunityDocumentConfiguration : IEntityTypeConfiguration<OpportunityDocument>
{
  public void Configure(EntityTypeBuilder<OpportunityDocument> builder)
  {
    builder.ToTable("opportunity_documents");

    builder.Property(document => document.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(document => document.Kind)
      .HasConversion(kind => kind.Value, value => SmartEnum<OpportunityDocumentKind, string>.FromValue(value))
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    builder.Property(document => document.Label)
      .HasMaxLength(DataSchemaConstants.DefaultNameLength)
      .IsRequired();

    builder.Property(document => document.Url)
      .HasMaxLength(DataSchemaConstants.UrlLength)
      .IsRequired();

    builder.Property(document => document.PublishedAt);

    builder.HasIndex("OpportunityId").HasDatabaseName("ix_opportunity_documents_opportunity");
  }
}
