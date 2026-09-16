using LicitaEdital.Application.Offerings.Create;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Offerings;

/// <summary>
/// Cria uma oferta. Responde **201** com a linha criada, no mesmo formato de `GET /offerings` — a
/// store do frontend insere a linha na lista sem recarregar.
/// </summary>
public class Create(IMediator mediator)
  : Endpoint<OfferingRequest,
             Results<Created<OfferingResponse>, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(OfferingsRoutes.Create);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("Offerings");
    Summary(s =>
    {
      s.Summary = "Cria uma oferta";
      s.Responses[201] = "Oferta criada";
      s.Responses[400] = "Campo inválido, ou já existe oferta com este nome";
    });
  }

  public override async Task<Results<Created<OfferingResponse>, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(OfferingRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreateOfferingCommand(request.ToPayload()),
      cancellationToken);

    return result.ToCreatedResult(
      offering => $"{OfferingsRoutes.List}/{offering.Id}",
      OfferingResponse.From);
  }
}
