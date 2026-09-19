using System.Reflection;
using LicitaEdital.Domain.Catalog.CompatibilityAggregate;
using LicitaEdital.Domain.Catalog.OpportunityAggregate;
using LicitaEdital.Data;
using LicitaEdital.Data.Catalog;

namespace LicitaEdital.Queries.Catalog;

/// <summary>
/// Lado de leitura do modulo Catalog.
///
/// Herda de <c>ReadOnlyModuleDbContext</c>, entao ja vem sem rastreamento, sem deteccao automatica
/// de alteracao, sem lazy loading e recusando <c>SaveChanges</c> — **nenhuma consulta deste projeto
/// escreve `AsNoTracking()`**, porque o rastreamento nunca chega a ser ligado.
///
/// O mapeamento vem das configuracoes do <see cref="CatalogDbContext"/>, no assembly de
/// persistencia: um mapeamento so para os dois lados.
/// </summary>
public class CatalogReadContext(DbContextOptions<CatalogReadContext> options)
  : ReadOnlyModuleDbContext(options)
{
  public DbSet<Opportunity> Opportunities => Set<Opportunity>();
  public DbSet<OpportunityCompatibility> Compatibilities => Set<OpportunityCompatibility>();

  /// <summary>
  /// O feed, com licitacao e compatibilidade ja juntas. Ver <see cref="OpportunityFeedRow"/> para o
  /// porque de ser SQL e nao LINQ.
  ///
  /// <b>Sem filtro de organizacao aqui</b>: quem consulta o aplica, onde ele fica visivel
  /// (<c>ListOpportunitiesQueryService</c>). O SQL recebe a organizacao como parametro porque ela
  /// entra na condicao do LEFT JOIN, e nao num WHERE — num WHERE, a licitacao sem nota sumiria.
  /// </summary>
  public DbSet<OpportunityFeedRow> Feed => Set<OpportunityFeedRow>();

  protected override string Schema => DataSchemaConstants.CatalogSchema;

  protected override string ConfigurationNamespace => typeof(CatalogDbContext).Namespace + ".Config";

  protected override Assembly ConfigurationAssembly => typeof(CatalogDbContext).Assembly;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<OpportunityFeedRow>(row =>
    {
      // `ToView(null)`: o tipo nao tem tabela nem visao propria — ele so' existe como resultado do
      // SQL que `ListOpportunitiesQueryService` executa por `FromSql`. Sem isto o EF exigiria uma
      // tabela chamada `OpportunityFeedRow`, que nao existe.
      row.HasNoKey().ToView(null);
      row.Property(feed => feed.Id).HasColumnName("id");
      row.Property(feed => feed.Title).HasColumnName("title");
      row.Property(feed => feed.Object).HasColumnName("object");
      row.Property(feed => feed.BuyerName).HasColumnName("buyer_name");
      row.Property(feed => feed.State).HasColumnName("state");
      row.Property(feed => feed.City).HasColumnName("city");
      row.Property(feed => feed.CityIbgeCode).HasColumnName("city_ibge_code");
      row.Property(feed => feed.ModalityCode).HasColumnName("modality_code");
      row.Property(feed => feed.ModalityLabel).HasColumnName("modality_label");
      row.Property(feed => feed.Status).HasColumnName("status");
      row.Property(feed => feed.EstimatedValueCents).HasColumnName("estimated_value_cents");
      row.Property(feed => feed.PublishedAt).HasColumnName("published_at");
      row.Property(feed => feed.ProposalDeadline).HasColumnName("proposal_deadline");
      row.Property(feed => feed.OfficialUrl).HasColumnName("official_url");
      row.Property(feed => feed.Source).HasColumnName("source");
      row.Property(feed => feed.CollectedAt).HasColumnName("collected_at");
      row.Property(feed => feed.Score).HasColumnName("score");
      row.Property(feed => feed.OfferingId).HasColumnName("offering_id");
      row.Property(feed => feed.MatchedTerms).HasColumnName("matched_terms").HasColumnType("text[]");
      row.Property(feed => feed.PositiveReasons).HasColumnName("positive_reasons").HasColumnType("text[]");
      row.Property(feed => feed.AttentionPoints).HasColumnName("attention_points").HasColumnType("text[]");
    });
  }

}
