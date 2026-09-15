using LicitaEdital.BuildingBlocks.Web.Defaults;
using LicitaEdital.UseCases.Identity.ResetPassword;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;

namespace LicitaEdital.Web.Auth;

/// <summary>
/// Redefine a senha pelo token do link (`AUTH-07`). Token invalido e token expirado respondem
/// igual — distinguir os dois diria a quem testa se a conta existe.
/// </summary>
[EnableRateLimiting(RateLimiterPolicies.Sensitive)]
public class ResetPassword(IMediator mediator)
  : Endpoint<ResetPasswordRequest, Results<NoContent, ValidationProblem>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(AuthRoutes.ResetPassword);
    AllowAnonymous();
    Tags("Auth");
    Summary(s =>
    {
      s.Summary = "Redefine a senha";
      s.Description = "Consome o token do link. Expiração e uso único são do provedor de identidade.";
    });
  }

  public override async Task<Results<NoContent, ValidationProblem>> ExecuteAsync(
    ResetPasswordRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ResetPasswordCommand(request.Token, request.Password),
      cancellationToken);

    return result.IsSuccess
      ? TypedResults.NoContent()
      : TypedResults.ValidationProblem(result.ValidationErrors
          .GroupBy(error => error.Identifier ?? string.Empty)
          .ToDictionary(group => group.Key, group => group.Select(e => e.ErrorMessage).ToArray()));
  }
}
