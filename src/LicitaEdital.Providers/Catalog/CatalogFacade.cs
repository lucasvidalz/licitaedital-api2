using LicitaEdital.Domain.Catalog.CompatibilityAggregate;
using LicitaEdital.Facade.Catalog;
using LicitaEdital.Domain.Catalog.OpportunityAggregate;
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

    var ids = opportunityIds.ToArray();

    var rows = await (
      from opportunity in _context.Opportunities
      where ids.Contains(opportunity.Id)
      join candidate in _context.Compatibilities.Where(c => c.OrganizationId == organizationId)
        on opportunity.Id equals candidate.OpportunityId into matches
      from compatibility in matches.DefaultIfEmpty()
      select new { opportunity, compatibility })
      .ToListAsync(cancellationToken);

    // A ordem do retorno segue a **da consulta**, nao a dos ids pedidos. Quem precisa de ordem
    // especifica — o feed de salvas ordena por data de salvamento — reordena do seu lado, que e'
    // onde o criterio existe.
    return [.. rows.Select(row => ToSummary(row.opportunity, row.compatibility))];
  }

  private static OpportunitySummary ToSummary(Opportunity opportunity,
    OpportunityCompatibility? compatibility)
    => new(
      opportunity.Id,
      opportunity.Title,
      opportunity.Object,
      opportunity.BuyerName,
      opportunity.State,
      opportunity.City,
      opportunity.CityIbgeCode,
      opportunity.Modality.Code,
      opportunity.Modality.Label,
      opportunity.Status.Value,
      opportunity.EstimatedValueCents,
      opportunity.PublishedAt,
      opportunity.ProposalDeadline,
      opportunity.OfficialUrl,
      opportunity.Source,
      opportunity.CollectedAt,
      ToCompatibility(compatibility));

  private static CompatibilitySummary ToCompatibility(OpportunityCompatibility? compatibility)
    => compatibility is null
      ? CompatibilitySummary.Unrated
      : new CompatibilitySummary(
          compatibility.Score?.Value,
          compatibility.OfferingId,
          [.. compatibility.MatchedTerms],
          [.. compatibility.PositiveReasons],
          [.. compatibility.AttentionPoints]);
}
