using LicitaEdital.Application.Engagement.SavedOpportunities.List;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.SavedOpportunities;

/// <summary>
/// As licitacoes salvas pela organizacao, mais recente primeiro, com a licitacao inteira embutida.
/// </summary>
public class List(IMediator mediator)
  : EndpointWithoutRequest<Results<Ok<IReadOnlyList<SavedOpportunityResponse>>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(SavedOpportunitiesRoutes.List);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("SavedOpportunities");
    Summary(s =>
    {
      s.Summary = "Licitações salvas";
      s.Description = "Ordenadas por data de salvamento, mais recente primeiro. " +
                      "Licitação despublicada na fonte some da lista.";
    });
  }

  public override async Task<Results<Ok<IReadOnlyList<SavedOpportunityResponse>>, NotFound, ProblemHttpResult>>
    ExecuteAsync(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ListSavedOpportunitiesQuery(), cancellationToken);

    return result.ToGetResult(views =>
      (IReadOnlyList<SavedOpportunityResponse>)[.. views.Select(SavedOpportunityResponse.From)]);
  }
}
