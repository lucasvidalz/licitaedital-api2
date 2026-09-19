using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.Facade.Catalog;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Catalog;
using Microsoft.EntityFrameworkCore;

namespace LicitaEdital.Providers.Catalog;

/// <summary>
/// A porta de Catalog para os outros modulos.
///
/// <para>
/// <b>Por que existe uma classe so para isto.</b> Engagement precisa da oportunidade para montar
/// `GET /saved-opportunities`. Sem a fachada, as saidas seriam: injetar <c>CatalogReadContext</c> em
/// Engagement — que e' o mesmo que dar a ele o schema `catalog` inteiro — ou um join entre schemas,
/// que a spec §4 proibe. Com a fachada, Catalog decide o que publica, e a mudanca de uma coluna
/// interna dele nao quebra ninguem.
/// </para>
/// </summary>
public class CatalogFacade(CatalogReadContext context) : ICatalogFacade
{
  private readonly CatalogReadContext _context = context;

  public async Task<IReadOnlyList<OpportunitySummary>> GetSummariesAsync(
    OrganizationId organizationId,
    IReadOnlyCollection<OpportunityId> opportunityIds,
    CancellationToken cancellationToken = default)
  {
    if (opportunityIds.Count == 0) return [];

    // Reusa a **mesma projecao do feed** (`FeedFor`), em vez de repetir o LEFT JOIN aqui. Alem de
    // nao duplicar o SQL, e' o que mantem os dois caminhos coerentes: a licitacao salva mostra
    // exatamente a nota que o feed mostra.
    //
    // O `join` em LINQ nao e' opcao: `Opportunity.Id` e `OpportunityCompatibility.OpportunityId` sao
    // value objects do Vogen, e o EF nao compara duas colunas convertidas entre si — a consulta
    // quebra em tempo de execucao. Ver a nota no `CLAUDE.md` do backend.
    var ids = opportunityIds.Select(id => id.Value).ToArray();

    var rows = await _context.FeedFor(organizationId)
      .Where(row => ids.Contains(row.Id))
      .ToListAsync(cancellationToken);

    // A ordem do retorno segue a **da consulta**, nao a dos ids pedidos. Quem precisa de ordem
    // especifica — o feed de salvas ordena por data de salvamento — reordena do seu lado, que e'
    // onde o criterio existe.
    return [.. rows.Select(ToSummary)];
  }

  private static OpportunitySummary ToSummary(OpportunityFeedRow row) => new(
    OpportunityId.From(row.Id),
    row.Title,
    row.Object,
    row.BuyerName,
    StateCode.Parse(row.State),
    row.City,
    row.CityIbgeCode,
    row.ModalityCode,
    row.ModalityLabel,
    row.Status,
    row.EstimatedValueCents,
    row.PublishedAt,
    row.ProposalDeadline,
    row.OfficialUrl,
    row.Source,
    row.CollectedAt,
    row.Score is null && row.OfferingId is null
      ? CompatibilitySummary.Unrated
      : new CompatibilitySummary(row.Score, row.OfferingId is { } id ? OfferingId.From(id) : null,
          row.MatchedTerms, row.PositiveReasons, row.AttentionPoints));
}
