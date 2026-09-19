using LicitaEdital.Application.Engagement.SavedOpportunities.Unsave;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Facade.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.SavedOpportunities;

/// <summary>
/// Remove o salvamento. **Idempotente**: dessalvar o que nao estava salvo responde 204, porque a
/// operacao atingiu o resultado desejado.
/// </summary>
public class Unsave(IMediator mediator)
  : EndpointWithoutRequest<Results<NoContent, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Delete(SavedOpportunitiesRoutes.Unsave);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("SavedOpportunities");
    Summary(s =>
    {
      s.Summary = "Remove o salvamento";
      s.Description = "O id é o da licitação, não o do registro de salvamento.";
      s.Responses[204] = "Removida — ou já não estava salva";
    });
  }

  public override async Task<Results<NoContent, NotFound, ProblemHttpResult>> ExecuteAsync(
    CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new UnsaveOpportunityCommand(OpportunityId.From(Route<Guid>("opportunityId"))),
      cancellationToken);

    return result.ToDeleteResult();
  }
}
