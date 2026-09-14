using LicitaEdital.Core.Catalog.CompatibilityAggregate;

namespace LicitaEdital.Infrastructure.Data.Catalog.Config;

public class OpportunityCompatibilityConfiguration : IEntityTypeConfiguration<OpportunityCompatibility>
{
  public void Configure(EntityTypeBuilder<OpportunityCompatibility> builder)
  {
    builder.ToTable("opportunity_compatibilities");

    builder.Property(compatibility => compatibility.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(compatibility => compatibility.OrganizationId).HasVogenConversion().IsRequired();
    builder.Property(compatibility => compatibility.OpportunityId).HasVogenConversion().IsRequired();

    // Referencia opaca a Offerings: id, sem FK. O modulo vizinho esta noutro schema, e uma FK
    // entre schemas e' exatamente o acoplamento que D-01 impede.
    builder.Property(compatibility => compatibility.OfferingId).HasVogenConversion();

    builder.Property(compatibility => compatibility.OfferingName)
      .HasMaxLength(DataSchemaConstants.DefaultNameLength);

    // Nulo e' estado real (`unrated`): organizacao sem oferta cadastrada nao tem nota.
    builder.Property(compatibility => compatibility.Score).HasVogenConversion();

    builder.PrimitiveCollection<List<string>>("_matchedTerms")
      .HasColumnName("matched_terms")
      .HasColumnType("text[]")
      .UsePropertyAccessMode(PropertyAccessMode.Field)
      .IsRequired();

    builder.PrimitiveCollection<List<string>>("_positiveReasons")
      .HasColumnName("positive_reasons")
      .HasColumnType("text[]")
      .UsePropertyAccessMode(PropertyAccessMode.Field)
      .IsRequired();

    builder.PrimitiveCollection<List<string>>("_attentionPoints")
      .HasColumnName("attention_points")
      .HasColumnType("text[]")
      .UsePropertyAccessMode(PropertyAccessMode.Field)
      .IsRequired();

    builder.Property(compatibility => compatibility.EngineVersion)
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    builder.Property(compatibility => compatibility.CalculatedAt).IsRequired();

    // Uma nota por (organizacao, licitacao) — guardamos a melhor oferta, que e' o que o contrato
    // expoe. Duas linhas dariam duas respostas para o mesmo card.
    builder.HasIndex(compatibility => new { compatibility.OrganizationId, compatibility.OpportunityId })
      .IsUnique()
      .HasDatabaseName("ux_opportunity_compatibilities_organization_opportunity");

    // Indice do `sort=score`: a ordenacao padrao do feed roda por organizacao, nota decrescente.
    builder.HasIndex(compatibility => new { compatibility.OrganizationId, compatibility.Score })
      .HasDatabaseName("ix_opportunity_compatibilities_organization_score");

    // Fila de recalculo: quem esta numa versao antiga do motor.
    builder.HasIndex(compatibility => compatibility.EngineVersion)
      .HasDatabaseName("ix_opportunity_compatibilities_engine_version");

    builder.UseXminAsConcurrencyToken();
  }
}
