using LicitaEdital.Domain.Catalog.OpportunityAggregate;
using LicitaEdital.Queries.Contracts.Catalog;

namespace LicitaEdital.Queries.Catalog;

/// <summary>
/// A consulta do feed. Filtra, ordena e pagina no banco, sobre a projecao
/// <see cref="OpportunityFeedRow"/>.
/// </summary>
public class ListOpportunitiesQueryService(CatalogReadContext context) : IListOpportunitiesQueryService
{
  private readonly CatalogReadContext _context = context;

  public async Task<PagedList<OpportunityListItemDto>> ListAsync(
    OrganizationId organizationId,
    ListOpportunitiesFilter filter,
    OpportunitySort sort,
    PageRequest page,
    CancellationToken cancellationToken = default)
  {
    var rows = _context.FeedFor(organizationId);

    rows = ApplyFilter(rows, filter);
    rows = ApplySort(rows, sort);

    var pageResult = await rows.ToPagedListAsync(page, cancellationToken);

    return pageResult.Map(ToDto);
  }


  private static IQueryable<OpportunityFeedRow> ApplyFilter(IQueryable<OpportunityFeedRow> rows,
    ListOpportunitiesFilter filter)
  {
    // ILIKE, e nao ToLower().Contains(): `LOWER(coluna) LIKE ...` cega qualquer indice comum da
    // coluna. Com volume real, o caminho daqui e' um indice GIN de trigrama sobre title/object.
    rows = rows.WhereIf(!string.IsNullOrWhiteSpace(filter.Search),
      row => EF.Functions.ILike(row.Title, $"%{filter.Search}%")
          || EF.Functions.ILike(row.Object, $"%{filter.Search}%"));

    rows = rows.WhereIf(filter.States.Count > 0, row => filter.States.Contains(row.State));

    // Compara o **codigo** da modalidade, nunca o rotulo — AD-030 do frontend nasceu exatamente de
    // comparar rotulo contra codigo.
    rows = rows.WhereIf(filter.Modalities.Count > 0,
      row => filter.Modalities.Contains(row.ModalityCode));

    // Licitacao sem valor estimado publicado **passa** pelo filtro de faixa: excluí-la esconderia
    // oportunidade real por ausencia de um dado que o orgao nao e' obrigado a publicar.
    rows = rows.WhereIf(filter.MinValueCents is not null,
      row => row.EstimatedValueCents == null || row.EstimatedValueCents >= filter.MinValueCents);

    rows = rows.WhereIf(filter.MaxValueCents is not null,
      row => row.EstimatedValueCents == null || row.EstimatedValueCents <= filter.MaxValueCents);

    return rows;
  }

  /// <summary>
  /// O <c>OrderBy(x =&gt; x.Campo == null)</c> antes do criterio real produz
  /// <c>ORDER BY (coluna IS NULL), coluna</c> — e' o jeito portavel de conseguir <c>NULLS LAST</c>.
  /// Sem ele, no PostgreSQL um <c>DESC</c> coloca NULL **primeiro**, e o feed abriria com as
  /// licitacoes sem nota no topo.
  /// </summary>
  private static IQueryable<OpportunityFeedRow> ApplySort(IQueryable<OpportunityFeedRow> rows,
    OpportunitySort sort)
  {
    if (sort == OpportunitySort.Deadline)
    {
      return rows
        .OrderBy(row => row.ProposalDeadline == null)
        .ThenBy(row => row.ProposalDeadline)
        .ThenByDescending(row => row.PublishedAt);
    }

    if (sort == OpportunitySort.PublishedAt)
    {
      return rows.OrderByDescending(row => row.PublishedAt).ThenByDescending(row => row.Id);
    }

    return rows
      .OrderBy(row => row.Score == null)
      .ThenByDescending(row => row.Score)
      .ThenByDescending(row => row.PublishedAt);
  }

  private static OpportunityListItemDto ToDto(OpportunityFeedRow row) => new(
    row.Id,
    row.Title,
    row.Object,
    row.BuyerName,
    row.State,
    row.City,
    row.CityIbgeCode,
    new ModalityDto(row.ModalityCode, row.ModalityLabel),
    row.Status,
    row.EstimatedValueCents,
    row.PublishedAt,
    row.ProposalDeadline,
    row.OfficialUrl,
    row.Source,
    row.CollectedAt,
    new CompatibilityDto(row.Score, row.OfferingId, row.MatchedTerms, row.PositiveReasons,
      row.AttentionPoints));
}
