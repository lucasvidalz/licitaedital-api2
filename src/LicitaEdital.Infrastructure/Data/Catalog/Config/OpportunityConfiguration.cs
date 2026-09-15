using LicitaEdital.Core.Catalog.OpportunityAggregate;
using LicitaEdital.Core.Shared;

namespace LicitaEdital.Infrastructure.Data.Catalog.Config;

public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
  public void Configure(EntityTypeBuilder<Opportunity> builder)
  {
    builder.ToTable("opportunities");

    builder.Property(opportunity => opportunity.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(opportunity => opportunity.Source)
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    builder.Property(opportunity => opportunity.ExternalReference)
      .HasMaxLength(DataSchemaConstants.DefaultNameLength)
      .IsRequired();

    builder.Property(opportunity => opportunity.Title)
      .HasMaxLength(DataSchemaConstants.DefaultNameLength * 2)
      .IsRequired();

    builder.Property(opportunity => opportunity.Object)
      .HasMaxLength(DataSchemaConstants.DefaultTextLength)
      .IsRequired();

    builder.Property(opportunity => opportunity.BuyerName)
      .HasMaxLength(DataSchemaConstants.DefaultNameLength)
      .IsRequired();

    builder.Property(opportunity => opportunity.ContractNumber)
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength);

    builder.Property(opportunity => opportunity.State)
      .HasStateCodeConversion()
      .IsRequired();

    builder.Property(opportunity => opportunity.City)
      .HasMaxLength(DataSchemaConstants.DefaultNameLength)
      .IsRequired();

    // 7 digitos do IBGE. String, nao int: o zero a esquerda de alguns codigos e' significativo.
    builder.Property(opportunity => opportunity.CityIbgeCode).HasMaxLength(7);

    // Modalidade e' codigo + rotulo juntos, na mesma linha — nao vale tabela propria: a fonte
    // publica os dois e nao ha entidade de modalidade com vida propria no produto.
    builder.OwnsOne(opportunity => opportunity.Modality, modality =>
    {
      modality.Property(value => value.Code)
        .HasColumnName("modality_code")
        .HasMaxLength(Modality.MaxCodeLength)
        .IsRequired();

      modality.Property(value => value.Label)
        .HasColumnName("modality_label")
        .HasMaxLength(Modality.MaxLabelLength)
        .IsRequired();
    });
    builder.Navigation(opportunity => opportunity.Modality).IsRequired();

    builder.Property(opportunity => opportunity.Status)
      .HasConversion(status => status.Value, value => SmartEnum<OpportunityStatus, string>.FromValue(value))
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    // Centavos em bigint. Nunca `numeric` com casas nem `double`: o contrato trafega inteiro
    // (`estimatedValueCents`) e converter na fronteira do banco reintroduziria arredondamento.
    builder.Property(opportunity => opportunity.EstimatedValueCents);

    builder.Property(opportunity => opportunity.PublishedAt).IsRequired();
    builder.Property(opportunity => opportunity.ProposalDeadline);
    builder.Property(opportunity => opportunity.CollectedAt).IsRequired();

    builder.Property(opportunity => opportunity.OfficialUrl)
      .HasMaxLength(DataSchemaConstants.UrlLength)
      .IsRequired();

    // Colecoes do agregado: campo privado, sem propriedade publica gravavel. A navegacao e'
    // declarada pelo nome do campo porque a convencao do EF nao descobre campo privado sozinha.
    builder.HasMany<OpportunityLineItem>("_items")
      .WithOne()
      .HasForeignKey("OpportunityId")
      .OnDelete(DeleteBehavior.Cascade);
    builder.Navigation("_items").UsePropertyAccessMode(PropertyAccessMode.Field);

    builder.HasMany<OpportunityDocument>("_documents")
      .WithOne()
      .HasForeignKey("OpportunityId")
      .OnDelete(DeleteBehavior.Cascade);
    builder.Navigation("_documents").UsePropertyAccessMode(PropertyAccessMode.Field);

    // Chave natural: e' o que torna a coleta idempotente. Reprocessar a mesma execucao atualiza a
    // linha existente em vez de criar uma segunda licitacao identica.
    builder.HasIndex(opportunity => new { opportunity.Source, opportunity.ExternalReference })
      .IsUnique()
      .HasDatabaseName("ux_opportunities_source_external_reference")
      .ActiveOnly();

    // Indices do feed: o filtro de `GET /opportunities` combina UF, modalidade e valor, e ordena
    // por prazo ou publicacao. `sort=score` nao passa por aqui — ordena pela projecao de
    // compatibilidade, que tem indice proprio.
    builder.HasIndex(opportunity => new { opportunity.State, opportunity.Status, opportunity.ProposalDeadline })
      .HasDatabaseName("ix_opportunities_state_status_deadline");

    builder.HasIndex(opportunity => opportunity.PublishedAt)
      .HasDatabaseName("ix_opportunities_published_at");

    builder.UseXminConcurrencyToken();
  }
}
