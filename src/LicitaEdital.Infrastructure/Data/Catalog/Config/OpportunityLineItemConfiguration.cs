using LicitaEdital.Core.Catalog.OpportunityAggregate;

namespace LicitaEdital.Infrastructure.Data.Catalog.Config;

public class OpportunityLineItemConfiguration : IEntityTypeConfiguration<OpportunityLineItem>
{
  public void Configure(EntityTypeBuilder<OpportunityLineItem> builder)
  {
    builder.ToTable("opportunity_line_items");

    builder.Property(item => item.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(item => item.Number).IsRequired();

    builder.Property(item => item.Description)
      .HasMaxLength(DataSchemaConstants.DefaultTextLength)
      .IsRequired();

    // Quantidade publicada pelo edital. `numeric(18,6)` — nunca `double`: fracao de unidade aparece
    // em edital de servico, e binario perderia o valor exato que a proposta vai multiplicar.
    builder.Property(item => item.Quantity)
      .HasPrecision(18, 6)
      .IsRequired();

    builder.Property(item => item.Unit)
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    builder.Property(item => item.UnitValueCents);
    builder.Property(item => item.TotalValueCents);

    builder.Property(item => item.CatalogCode).HasMaxLength(DataSchemaConstants.DefaultCodeLength);

    // Numero do item e' unico dentro da licitacao — e' assim que o edital o referencia.
    builder.HasIndex("OpportunityId", nameof(OpportunityLineItem.Number))
      .IsUnique()
      .HasDatabaseName("ux_opportunity_line_items_opportunity_number");
  }
}
