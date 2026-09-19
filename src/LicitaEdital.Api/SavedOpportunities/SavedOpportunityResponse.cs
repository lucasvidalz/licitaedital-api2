using LicitaEdital.Api.Opportunities;
using LicitaEdital.Application.Engagement.SavedOpportunities;
using LicitaEdital.Facade.Catalog;

namespace LicitaEdital.Api.SavedOpportunities;

/// <summary>
/// `SavedOpportunityApiItem` do contrato: a data do salvamento e a **licitacao inteira** embutida,
/// no mesmo formato de `GET /opportunities` — o frontend reusa `OpportunityApiItem` por importacao,
/// sem redeclarar campo.
/// </summary>
public sealed record SavedOpportunityResponse(DateTimeOffset SavedAt, OpportunityResponse Opportunity)
{
  public static SavedOpportunityResponse From(SavedOpportunityView view)
    => new(view.SavedAt, FromSummary(view.Opportunity));

  /// <summary>
  /// Traduz o **contrato entre modulos** (<see cref="OpportunitySummary"/>) para o **contrato com o
  /// cliente** (<see cref="OpportunityResponse"/>). Os dois coincidem hoje e sao tipos distintos de
  /// proposito: o dia em que a tela pedir um campo novo, so' um dos dois muda.
  /// </summary>
  private static OpportunityResponse FromSummary(OpportunitySummary summary) => new(
    summary.Id.Value,
    summary.Title,
    summary.Object,
    summary.BuyerName,
    summary.State.Value,
    summary.City,
    summary.CityIbgeCode,
    new ModalityResponse(summary.ModalityCode, summary.ModalityLabel),
    summary.Status,
    summary.EstimatedValueCents,
    summary.PublishedAt,
    summary.ProposalDeadline,
    summary.OfficialUrl,
    summary.Source,
    summary.CollectedAt,
    new CompatibilityResponse(
      summary.Compatibility.Score,
      summary.Compatibility.OfferingId?.Value,
      summary.Compatibility.MatchedTerms,
      summary.Compatibility.PositiveReasons,
      summary.Compatibility.AttentionPoints));
}
