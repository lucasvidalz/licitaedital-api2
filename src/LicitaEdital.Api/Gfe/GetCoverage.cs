using LicitaEdital.Application.Collections.Coverage.Get;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Gfe;

/// <summary>
/// Cobertura do radar: ate onde o coletor busca.
///
/// <para>
/// <b>Area `manager`, sem permissao nomeada.</b> A rota `/gfe/settings` do Angular nao declara
/// `data.permissions` (`gfe.routes.ts`), diferente de `/gfe/users` e `/gfe/collections` — quem entra
/// na area de gerenciamento configura a cobertura. Espelhar aqui o que a rota do cliente declara e'
/// o que impede as duas divergirem.
/// </para>
///
/// <b>Nunca responde 404</b>: sem cobertura gravada, o padrao.
/// </summary>
public class GetCoverage(IMediator mediator)
  : EndpointWithoutRequest<Results<Ok<CoverageSettingsResponse>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(GfeRoutes.Settings);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Manager));
    Tags("Gfe");
    Summary(s => s.Summary = "Cobertura do radar");
  }

  public override async Task<Results<Ok<CoverageSettingsResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetCoverageSettingsQuery(), cancellationToken);

    return result.ToGetResult(CoverageSettingsResponse.From);
  }
}
