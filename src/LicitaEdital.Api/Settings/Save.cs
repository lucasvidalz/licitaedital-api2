using LicitaEdital.Application.Engagement.Settings.Save;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Settings;

/// <summary>Substitui as preferencias inteiras. Upsert idempotente, como `/company-profile`.</summary>
public class Save(IMediator mediator)
  : Endpoint<SettingsRequest,
             Results<Ok<SettingsResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Put(SettingsRoutes.Save);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("Settings");
    Summary(s => s.Summary = "Grava as preferências de alerta");
  }

  public override async Task<Results<Ok<SettingsResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(SettingsRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new SaveSettingsCommand(
        request.AlertTypes.NewCompatibleOpportunity,
        request.AlertTypes.ApproachingDeadline,
        request.AlertTypes.DailySummary,
        request.Frequency,
        request.Filter.States,
        request.Filter.Modalities,
        request.Filter.ValueRange),
      cancellationToken);

    return result.ToUpdateResult(SettingsResponse.From);
  }
}
