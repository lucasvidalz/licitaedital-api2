using LicitaEdital.Core.Catalog.CompatibilityAggregate;
using LicitaEdital.Core.Catalog.OpportunityAggregate;
using LicitaEdital.UseCases.Catalog.Opportunities.List;

namespace LicitaEdital.Query.Catalog;

/// <summary>
/// A consulta do feed. Filtra, ordena, pagina e junta a compatibilidade da organizacao numa ida so
/// ao banco.
/// </summary>
public class ListOpportunitiesQueryService(CatalogReadContext context) : IListOpportunitiesQueryService
{
  private readonly CatalogReadContext _context = context;

  public async Task<PagedResult<OpportunityListItemDto>> ListAsync(
    OrganizationId organizationId,
    ListOpportunitiesFilter filter,
    OpportunitySort sort,
    PageRequest page,
    CancellationToken cancellationToken = default)
  {
    // LEFT JOIN com a projecao de compatibilidade **da organizacao pedida**. O filtro por
    // organizacao vai na condicao do join, nao num WHERE depois: num WHERE, a linha sem
    // compatibilidade viraria NULL e seria descartada, e o feed sumiria inteiro para quem ainda nao
    // cadastrou oferta.
    var rows = from opportunity in _context.Opportunities
               join candidate in _context.Compatibilities.Where(c => c.OrganizationId == organizationId)
                 on opportunity.Id equals candidate.OpportunityId into matches
               from compatibility in matches.DefaultIfEmpty()
               select new Row(opportunity, compatibility);

    rows = ApplyFilter(rows, filter);
    rows = ApplySort(rows, sort);

    var pageResult = await rows.ToPagedResultAsync(page, cancellationToken);

    return pageResult.Map(ToDto);
  }

  private static IQueryable<Row> ApplyFilter(IQueryable<Row> rows, ListOpportunitiesFilter filter)
  {
    // ILIKE, e nao ToLower().Contains(): `LOWER(coluna) LIKE ...` cega qualquer indice comum da
    // coluna. Com volume real, o caminho daqui e' um indice GIN de trigrama sobre title/object.
    rows = rows.WhereIf(!string.IsNullOrWhiteSpace(filter.Search),
      row => EF.Functions.ILike(row.Opportunity.Title, $"%{filter.Search}%")
          || EF.Functions.ILike(row.Opportunity.Object, $"%{filter.Search}%"));

    rows = rows.WhereIf(filter.States.Count > 0,
      row => filter.States.Contains(row.Opportunity.State.Value));

    // Compara o **codigo** da modalidade, nunca o rotulo — AD-030 do frontend nasceu exatamente de
    // comparar rotulo contra codigo.
    rows = rows.WhereIf(filter.Modalities.Count > 0,
      row => filter.Modalities.Contains(row.Opportunity.Modality.Code));

    // Licitacao sem valor estimado publicado **passa** pelo filtro de faixa: excluí-la esconderia
    // oportunidade real por ausencia de um dado que o orgao nao e' obrigado a publicar.
    rows = rows.WhereIf(filter.MinValueCents is not null,
      row => row.Opportunity.EstimatedValueCents == null
          || row.Opportunity.EstimatedValueCents >= filter.MinValueCents);

    rows = rows.WhereIf(filter.MaxValueCents is not null,
      row => row.Opportunity.EstimatedValueCents == null
          || row.Opportunity.EstimatedValueCents <= filter.MaxValueCents);

    return rows;
  }

  /// <summary>
  /// O `OrderBy(x => x.Campo == null)` antes do criterio real produz `ORDER BY (coluna IS NULL),
  /// coluna` — e' o jeito portavel de conseguir `NULLS LAST`. Sem ele, no PostgreSQL um `DESC`
  /// coloca NULL **primeiro**, e o feed abriria com as licitacoes sem nota no topo.
  /// </summary>
  private static IQueryable<Row> ApplySort(IQueryable<Row> rows, OpportunitySort sort) => sort switch
  {
    OpportunitySort.Score => rows
      .OrderBy(row => row.Compatibility == null)
      .ThenByDescending(row => row.Compatibility!.Score)
      .ThenByDescending(row => row.Opportunity.PublishedAt),

    OpportunitySort.Deadline => rows
      .OrderBy(row => row.Opportunity.ProposalDeadline == null)
      .ThenBy(row => row.Opportunity.ProposalDeadline),

    _ => rows.OrderByDescending(row => row.Opportunity.PublishedAt)
  };

  private static OpportunityListItemDto ToDto(Row row) => new(
    row.Opportunity.Id.Value,
    row.Opportunity.Title,
    row.Opportunity.Object,
    row.Opportunity.BuyerName,
    row.Opportunity.State.Value,
    row.Opportunity.City,
    row.Opportunity.CityIbgeCode,
    new ModalityDto(row.Opportunity.Modality.Code, row.Opportunity.Modality.Label),
    row.Opportunity.Status.Value,
    row.Opportunity.EstimatedValueCents,
    row.Opportunity.PublishedAt,
    row.Opportunity.ProposalDeadline,
    row.Opportunity.OfficialUrl,
    row.Opportunity.Source,
    row.Opportunity.CollectedAt,
    ToCompatibilityDto(row.Compatibility));

  private static CompatibilityDto ToCompatibilityDto(OpportunityCompatibility? compatibility)
    => compatibility is null
      ? new CompatibilityDto(null, null, [], [], [])
      : new CompatibilityDto(
          compatibility.Score?.Value,
          compatibility.OfferingId?.Value,
          [.. compatibility.MatchedTerms],
          [.. compatibility.PositiveReasons],
          [.. compatibility.AttentionPoints]);

  /// <summary>
  /// Linha intermediaria da consulta. Carrega as **entidades**, e nao colunas soltas, de proposito:
  /// projetar value object do Vogen dentro de um `Select` traduzido para SQL e' terreno movedico no
  /// EF. O custo e' trazer a linha inteira de `opportunities` — que a tela usa quase por completo,
  /// e cujas colecoes (itens, documentos) estao em outras tabelas e nao vem junto.
  /// </summary>
  private sealed record Row(Opportunity Opportunity, OpportunityCompatibility? Compatibility);
}
