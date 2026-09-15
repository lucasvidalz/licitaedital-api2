using LicitaEdital.BuildingBlocks.Web.Defaults;
using LicitaEdital.UseCases.Identity.ConfirmEmail;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;

namespace LicitaEdital.Web.Auth;

/// <summary>
/// Confirma o e-mail pelo token do link (`AUTH-08`). **Nao autentica**: quem abre o link pode estar
/// noutro navegador, e confirmar e-mail nao prova posse de senha.
/// </summary>
[EnableRateLimiting(RateLimiterPolicies.Sensitive)]
public class ConfirmEmail(IMediator mediator)
  : Endpoint<ConfirmEmailRequest, Results<NoContent, ValidationProblem>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(AuthRoutes.ConfirmEmail);
    AllowAnonymous();
    Tags("Auth");
    Summary(s => s.Summary = "Confirma o e-mail da conta");
  }

  public override async Task<Results<NoContent, ValidationProblem>> ExecuteAsync(
    ConfirmEmailRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ConfirmEmailCommand(request.Token), cancellationToken);

    return result.IsSuccess
      ? TypedResults.NoContent()
      : TypedResults.ValidationProblem(result.ValidationErrors
          .GroupBy(error => error.Identifier ?? string.Empty)
          .ToDictionary(group => group.Key, group => group.Select(e => e.ErrorMessage).ToArray()));
  }
}
