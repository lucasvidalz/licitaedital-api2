using LicitaEdital.Application.Collections.Coverage.Save;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Gfe;

/// <summary>Substitui a cobertura do radar. Upsert idempotente sobre um singleton.</summary>
public class SaveCoverage(IMediator mediator)
  : Endpoint<CoverageSettingsRequest,
             Results<Ok<CoverageSettingsResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Put(GfeRoutes.Settings);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Manager));
    Tags("Gfe");
    Summary(s => s.Summary = "Grava a cobertura do radar");
  }

  public override async Task<Results<Ok<CoverageSettingsResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CoverageSettingsRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new SaveCoverageSettingsCommand(request.AttendedStates, request.ValueRange), cancellationToken);

    return result.ToUpdateResult(CoverageSettingsResponse.From);
  }
}
