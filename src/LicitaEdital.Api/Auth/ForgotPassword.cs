using LicitaEdital.BuildingBlocks.Web.Defaults;
using LicitaEdital.Queries.Contracts.Identity.ForgotPassword;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;

namespace LicitaEdital.Api.Auth;

/// <summary>
/// Dispara o link de redefinicao (`FEAT-12.3`, `AUTH-06`).
///
/// **Sempre 204**, exista o e-mail ou nao. E o envio e' assincrono no caso de uso, para que o
/// **tempo** de resposta tambem nao denuncie — mensagem generica com latencia diferente e' protecao
/// cosmetica, visivel na aba Network.
/// </summary>
[EnableRateLimiting(RateLimiterPolicies.Sensitive)]
public class ForgotPassword(IMediator mediator)
  : Endpoint<ForgotPasswordRequest, Results<NoContent, ValidationProblem>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(AuthRoutes.ForgotPassword);
    AllowAnonymous();
    Tags("Auth");
    Summary(s =>
    {
      s.Summary = "Envia o link de redefinição de senha";
      s.Description = "Responde 204 sempre — não revela se o e-mail existe.";
    });
  }

  public override async Task<Results<NoContent, ValidationProblem>> ExecuteAsync(
    ForgotPasswordRequest request, CancellationToken cancellationToken)
  {
    await _mediator.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
    return TypedResults.NoContent();
  }
}
