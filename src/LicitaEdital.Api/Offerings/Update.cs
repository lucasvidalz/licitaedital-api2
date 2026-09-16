using LicitaEdital.Application.Offerings.Update;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Facade.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Offerings;

/// <summary>
/// Substitui uma oferta inteira. Oferta de outra organizacao responde **404**, nao 403: 403
/// confirmaria que aquele id existe em algum lugar (spec §16).
/// </summary>
public class Update(IMediator mediator)
  : Endpoint<UpdateOfferingRequest,
             Results<Ok<OfferingResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Put(OfferingsRoutes.Update);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("Offerings");
    Summary(s =>
    {
      s.Summary = "Substitui uma oferta";
      s.Responses[200] = "Oferta gravada";
      s.Responses[404] = "Não existe nesta organização";
    });
  }

  public override async Task<Results<Ok<OfferingResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(UpdateOfferingRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new UpdateOfferingCommand(OfferingId.From(request.Id), request.ToPayload()),
      cancellationToken);

    return result.ToUpdateResult(OfferingResponse.From);
  }
}
