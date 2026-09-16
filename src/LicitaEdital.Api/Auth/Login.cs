using LicitaEdital.BuildingBlocks.Auth;
using LicitaEdital.BuildingBlocks.Web.Defaults;
using LicitaEdital.Application.Identity.Login;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;

namespace LicitaEdital.Api.Auth;

/// <summary>
/// Autentica e emite o cookie de sessao (`AUTH-01`).
///
/// O caso de uso valida a credencial; **o cookie e' emitido aqui**, porque sessao e' assunto de
/// HTTP e nao do dominio. A separacao e' o que permite testar o login sem `HttpContext`.
/// </summary>
[EnableRateLimiting(RateLimiterPolicies.Sensitive)]
public class Login(IMediator mediator)
  : Endpoint<LoginRequest, Results<Ok<AuthUserResponse>, UnauthorizedHttpResult, ValidationProblem>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(AuthRoutes.Login);
    AllowAnonymous();
    Tags("Auth");
    Summary(s =>
    {
      s.Summary = "Autentica e abre a sessão";
      s.Description = "Credencial inválida, conta bloqueada e vínculo suspenso respondem igual: 401.";
      s.Responses[200] = "Autenticado; o cookie de sessão vai no Set-Cookie";
      s.Responses[401] = "Credencial inválida";
      s.Responses[429] = "Tentativas demais — limite por IP";
    });
  }

  public override async Task<Results<Ok<AuthUserResponse>, UnauthorizedHttpResult, ValidationProblem>>
    ExecuteAsync(LoginRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new LoginCommand(request.Email, request.Password),
      cancellationToken);

    if (!result.IsSuccess) return TypedResults.Unauthorized();

    var user = result.Value;
    await HttpContext.SignInSessionAsync(SessionPrincipal.Create(
      user.Id, user.Email, user.DisplayName, TenantOf(user), user.Area, user.Permissions));

    return TypedResults.Ok(AuthUserResponse.From(user));
  }

  /// <summary>
  /// O tenant vem do vinculo resolvido pelo caso de uso. Hoje um usuario tem um vinculo ativo; o
  /// dia em que tiver mais de um, esta linha vira a escolha de organizacao — e e' aqui que ela
  /// aparece, nao espalhada.
  /// </summary>
  private static Guid TenantOf(Queries.Contracts.Identity.AuthenticatedUserDto user) => user.TenantId;
}
