using LicitaEdital.UseCases.Identity.Me;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Web.Auth;

/// <summary>
/// Resolve a sessao no boot do frontend (`AUTH-10`). Responde **401 quando nao ha sessao valida**,
/// e o `AuthStore` trata isso como visitante anonimo, nao como erro.
/// </summary>
public class Me(IMediator mediator) : EndpointWithoutRequest<Results<Ok<AuthUserResponse>, UnauthorizedHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(AuthRoutes.Me);
    // Anonimo no pipeline, autenticado na regra: sem `AllowAnonymous` o proprio middleware
    // devolveria 401 antes do handler, e o resultado seria o mesmo — mas por um caminho que nao
    // passa pelo caso de uso e nao revalida o vinculo. Quem decide e' `GetCurrentUserHandler`.
    AllowAnonymous();
    Tags("Auth");
    Summary(s =>
    {
      s.Summary = "Sessão atual";
      s.Description = "Devolve o usuário da sessão, ou 401 quando não há sessão válida.";
      s.Responses[200] = "Sessão válida";
      s.Responses[401] = "Sem sessão, sessão expirada ou vínculo suspenso";
    });
  }

  public override async Task<Results<Ok<AuthUserResponse>, UnauthorizedHttpResult>> ExecuteAsync(
    CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);

    return result.IsSuccess
      ? TypedResults.Ok(AuthUserResponse.From(result.Value))
      : TypedResults.Unauthorized();
  }
}
