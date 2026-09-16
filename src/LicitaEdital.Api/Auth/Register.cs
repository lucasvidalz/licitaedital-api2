using LicitaEdital.BuildingBlocks.Auth;
using LicitaEdital.BuildingBlocks.Web.Defaults;
using LicitaEdital.Queries.Contracts.Identity.Register;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;

namespace LicitaEdital.Api.Auth;

/// <summary>
/// Cria a conta e **autentica na hora**, sem exigir confirmacao de e-mail (`FEAT-12.2`,
/// `AUTH-04`). O e-mail de confirmacao sai em paralelo e nao bloqueia nada.
/// </summary>
[EnableRateLimiting(RateLimiterPolicies.Sensitive)]
public class Register(IMediator mediator)
  : Endpoint<RegisterRequest, Results<Ok<AuthUserResponse>, ValidationProblem>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(AuthRoutes.Register);
    AllowAnonymous();
    Tags("Auth");
    Summary(s =>
    {
      s.Summary = "Cria a conta e abre a sessão";
      s.Description = "Não exige confirmação de e-mail para entrar — é decisão de produto, não descuido.";
      s.Responses[200] = "Conta criada e autenticada";
      s.Responses[400] = "E-mail já cadastrado ou senha fora da política";
      s.Responses[429] = "Tentativas demais — limite por IP";
    });
  }

  public override async Task<Results<Ok<AuthUserResponse>, ValidationProblem>> ExecuteAsync(
    RegisterRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new RegisterCommand(request.Email, request.Password, request.DisplayName), cancellationToken);

    if (!result.IsSuccess)
    {
      return TypedResults.ValidationProblem(result.ValidationErrors
        .GroupBy(error => error.Identifier ?? string.Empty)
        .ToDictionary(group => group.Key, group => group.Select(e => e.ErrorMessage).ToArray()));
    }

    var user = result.Value;
    await HttpContext.SignInSessionAsync(SessionPrincipal.Create(
      user.Id, user.Email, user.DisplayName, user.TenantId, user.Area, user.Permissions));

    return TypedResults.Ok(AuthUserResponse.From(user));
  }
}
