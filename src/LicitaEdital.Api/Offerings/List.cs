using LicitaEdital.Application.Offerings.List;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Offerings;

/// <summary>
/// Todas as ofertas da organizacao da sessao.
///
/// Sem paginacao e sem envelope: o contrato devolve `OfferingApiItem[]` puro
/// (`offerings-api.service.ts`), e a ordenacao final e' refeita no cliente. Uma empresa tem dezenas
/// de ofertas, nao milhares.
/// </summary>
public class List(IMediator mediator)
  : EndpointWithoutRequest<Results<Ok<IReadOnlyList<OfferingResponse>>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(OfferingsRoutes.List);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("Offerings");
    Summary(s =>
    {
      s.Summary = "Lista as ofertas da empresa";
      s.Responses[200] = "Lista, possivelmente vazia";
    });
  }

  public override async Task<Results<Ok<IReadOnlyList<OfferingResponse>>, NotFound, ProblemHttpResult>>
    ExecuteAsync(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ListOfferingsQuery(), cancellationToken);

    return result.ToGetResult(offerings =>
      (IReadOnlyList<OfferingResponse>)[.. offerings.Select(OfferingResponse.From)]);
  }
}
