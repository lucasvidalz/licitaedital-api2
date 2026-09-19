using LicitaEdital.Application.Engagement.SavedOpportunities.Save;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Facade.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.SavedOpportunities;

/// <summary>
/// Salva uma licitacao.
///
/// <para>
/// **Responde 200, nao 201, e e' idempotente.** O botao da tela e' um alternador: salvar de novo
/// devolve a mesma linha em vez de criar a segunda ou falhar. Um 201 prometeria criacao a cada
/// chamada, o que deixaria de ser verdade na segunda.
/// </para>
/// </summary>
public class Save(IMediator mediator)
  : Endpoint<SaveOpportunityRequest,
             Results<Ok<SavedOpportunityResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(SavedOpportunitiesRoutes.Save);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("SavedOpportunities");
    Summary(s =>
    {
      s.Summary = "Salva uma licitação";
      s.Responses[200] = "Salva — ou já estava salva, mesma resposta";
      s.Responses[404] = "Licitação não existe";
    });
  }

  public override async Task<Results<Ok<SavedOpportunityResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(SaveOpportunityRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new SaveOpportunityCommand(OpportunityId.From(request.OpportunityId)), cancellationToken);

    return result.ToUpdateResult(SavedOpportunityResponse.From);
  }
}
