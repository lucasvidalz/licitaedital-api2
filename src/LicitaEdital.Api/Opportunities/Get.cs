using LicitaEdital.Application.Catalog.Opportunities.Details;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Facade.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Opportunities;

/// <summary>
/// Detalhe da licitacao, com itens, documentos e a compatibilidade da organizacao da sessao.
///
/// <para>
/// <b>404 aqui e' erro de verdade</b>, diferente de `GET /company-profile`: o id veio de um link que
/// o proprio feed produziu, entao ausencia significa id inventado ou licitacao despublicada — e a
/// tela mostra estado de erro, nao formulario vazio.
/// </para>
/// </summary>
public class Get(IMediator mediator)
  : EndpointWithoutRequest<Results<Ok<OpportunityDetailResponse>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    base.Get(OpportunitiesRoutes.Get);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("Opportunities");
    Summary(s =>
    {
      s.Summary = "Detalhe de uma licitação";
      s.Responses[200] = "Licitação encontrada";
      s.Responses[404] = "Não existe ou foi despublicada";
    });
  }

  public override async Task<Results<Ok<OpportunityDetailResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new GetOpportunityQuery(OpportunityId.From(Route<Guid>("id"))), cancellationToken);

    return result.ToGetResult(OpportunityDetailResponse.From);
  }
}
