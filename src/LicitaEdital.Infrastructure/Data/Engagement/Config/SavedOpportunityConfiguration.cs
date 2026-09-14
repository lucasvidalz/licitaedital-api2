using LicitaEdital.Core.Engagement.SavedOpportunityAggregate;

namespace LicitaEdital.Infrastructure.Data.Engagement.Config;

public class SavedOpportunityConfiguration : IEntityTypeConfiguration<SavedOpportunity>
{
  public void Configure(EntityTypeBuilder<SavedOpportunity> builder)
  {
    builder.ToTable("saved_opportunities");

    builder.Property(saved => saved.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(saved => saved.OrganizationId).HasVogenConversion().IsRequired();

    // Referencia opaca a Catalog: id, sem FK entre schemas.
    builder.Property(saved => saved.OpportunityId).HasVogenConversion().IsRequired();

    builder.Property(saved => saved.SavedBy).HasVogenConversion().IsRequired();
    builder.Property(saved => saved.SavedAt).IsRequired();

    // Salvar duas vezes e' idempotente. E' esta unicidade — e nao uma checagem na aplicacao — que
    // garante isso sob concorrencia: dois cliques simultaneos no mesmo card colidem aqui.
    builder.HasIndex(saved => new { saved.OrganizationId, saved.OpportunityId })
      .IsUnique()
      .HasDatabaseName("ux_saved_opportunities_organization_opportunity");

    // `GET /saved-opportunities` lista por organizacao, mais recentes primeiro.
    builder.HasIndex(saved => new { saved.OrganizationId, saved.SavedAt })
      .HasDatabaseName("ix_saved_opportunities_organization_saved_at");
  }
}
