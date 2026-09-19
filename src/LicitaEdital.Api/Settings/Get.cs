using LicitaEdital.Application.Engagement.Settings.Get;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Settings;

/// <summary>
/// Preferencias de alerta da organizacao.
///
/// <b>Nunca responde 404</b>, diferente de `/company-profile`: organizacao sem preferencia gravada
/// recebe o padrao. A tela de configuracoes nao tem estado "ainda nao cadastrado".
/// </summary>
public class Get(IMediator mediator)
  : EndpointWithoutRequest<Results<Ok<SettingsResponse>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    base.Get(SettingsRoutes.Get);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("Settings");
    Summary(s =>
    {
      s.Summary = "Preferências de alerta";
      s.Responses[200] = "Preferências gravadas, ou o padrão quando ainda não há";
    });
  }

  public override async Task<Results<Ok<SettingsResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
    CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetSettingsQuery(), cancellationToken);

    return result.ToGetResult(SettingsResponse.From);
  }
}
