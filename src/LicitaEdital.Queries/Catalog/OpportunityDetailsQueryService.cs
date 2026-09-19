using LicitaEdital.Domain.Catalog.CompatibilityAggregate;
using LicitaEdital.Domain.Catalog.OpportunityAggregate;
using LicitaEdital.Domain.Catalog.OpportunityAggregate.Specifications;
using LicitaEdital.Queries.Contracts.Catalog;

namespace LicitaEdital.Queries.Catalog;

/// <summary>
/// O detalhe: a licitacao com itens e documentos, mais a compatibilidade da organizacao.
/// </summary>
public class OpportunityDetailsQueryService(CatalogReadContext context)
  : IOpportunityDetailsQueryService
{
  private readonly CatalogReadContext _context = context;

  public async Task<OpportunityDetailDto?> FindAsync(OrganizationId organizationId,
    OpportunityId opportunityId, CancellationToken cancellationToken = default)
  {
    // `AsSplitQuery` aqui, e nao global: sao duas colecoes no mesmo `Include`, e sem a divisao o
    // PostgreSQL devolveria o produto cartesiano entre itens e documentos — a licitacao com 40 itens
    // e 5 anexos viraria 200 linhas para montar 45 objetos.
    // `Include` por **nome de campo**: `Items` e `Documents` sao projecoes de leitura marcadas
    // `Ignore` no mapeamento, e as navegacoes de verdade sao os campos `_items` e `_documents`. A
    // forma com lambda compila e quebra em tempo de execucao.
    var opportunity = await _context.Opportunities
      .Include(OpportunityWithItemsSpec.ItemsNavigation)
      .Include(OpportunityWithItemsSpec.DocumentsNavigation)
      .AsSplitQuery()
      .FirstOrDefaultAsync(candidate => candidate.Id == opportunityId, cancellationToken);

    if (opportunity is null) return null;

    // Consulta separada, e nao `join`: a compatibilidade e' opcional (`unrated` e' estado real) e
    // juntar com `LEFT JOIN` multiplicaria de novo as duas colecoes ja incluidas acima.
    var compatibility = await _context.Compatibilities
      .FirstOrDefaultAsync(candidate => candidate.OpportunityId == opportunityId
                                     && candidate.OrganizationId == organizationId,
        cancellationToken);

    return ToDto(opportunity, compatibility);
  }

  private static OpportunityDetailDto ToDto(Opportunity opportunity,
    OpportunityCompatibility? compatibility) => new(
    opportunity.Id.Value,
    opportunity.Title,
    opportunity.Object,
    opportunity.BuyerName,
    // O contrato declara `contractNumber: string`, nao anulavel: a tela imprime o campo direto. A
    // fonte nem sempre publica, entao a ausencia vira string vazia aqui — e nao `null` num campo que
    // o TypeScript garante existir.
    opportunity.ContractNumber ?? string.Empty,
    opportunity.State.Value,
    opportunity.City,
    opportunity.CityIbgeCode,
    new ModalityDto(opportunity.Modality.Code, opportunity.Modality.Label),
    opportunity.EstimatedValueCents,
    opportunity.PublishedAt,
    opportunity.ProposalDeadline,
    opportunity.OfficialUrl,
    opportunity.Source,
    opportunity.CollectedAt,
    [.. opportunity.Items
        .OrderBy(item => item.Number)
        .Select(item => new OpportunityLineItemDto(item.Number, item.Description, item.Quantity,
          item.Unit, item.UnitValueCents, item.TotalValueCents, item.CatalogCode))],
    [.. opportunity.Documents
        .OrderByDescending(document => document.PublishedAt)
        .Select(document => new OpportunityDocumentDto(document.Kind.Value, document.Label,
          document.Url, document.PublishedAt))],
    ToCompatibilityDto(compatibility));

  private static OpportunityDetailCompatibilityDto ToCompatibilityDto(
    OpportunityCompatibility? compatibility)
    => compatibility is null
      ? new OpportunityDetailCompatibilityDto(null, null, null, [], [], [])
      : new OpportunityDetailCompatibilityDto(
          compatibility.Score?.Value,
          compatibility.OfferingId?.Value,
          compatibility.OfferingName,
          [.. compatibility.MatchedTerms],
          [.. compatibility.PositiveReasons],
          [.. compatibility.AttentionPoints]);
}
