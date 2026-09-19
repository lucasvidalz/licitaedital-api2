using LicitaEdital.Application.Engagement.Subscription.Get;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Subscription;

/// <summary>
/// Plano vigente da organizacao.
///
/// <para>
/// <b>Devolve so' `{ planId }`.</b> Preco, recursos e ciclo sao catalogo estatico no cliente e nao
/// atravessam a API — acrescentar campo que a tela nao usa e' superficie de dado a mais sem ganho
/// (SEC-64).
/// </para>
///
/// <para>
/// <b>Nao ha `PUT`</b>: a troca de plano corre pelo time comercial (`AD-039` do frontend). Expor uma
/// rota de troca criaria um fluxo que nao termina.
/// </para>
/// </summary>
public class Get(IMediator mediator)
  : EndpointWithoutRequest<Results<Ok<SubscriptionResponse>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    base.Get("/subscription");
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("Subscription");
    Summary(s =>
    {
      s.Summary = "Plano vigente";
      s.Responses[404] = "Nenhuma assinatura registrada — a tela trata como dado ausente";
    });
  }

  public override async Task<Results<Ok<SubscriptionResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetSubscriptionQuery(), cancellationToken);

    return result.ToGetResult(planId => new SubscriptionResponse(planId));
  }
}

/// <summary>`SubscriptionApiItem` do contrato.</summary>
public sealed record SubscriptionResponse(string PlanId);
