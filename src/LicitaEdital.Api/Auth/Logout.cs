using LicitaEdital.BuildingBlocks.Auth;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Auth;

/// <summary>
/// Encerra a sessao no servidor e remove o cookie (`AUTH-13`, SEC-36).
///
/// Responde 204 mesmo sem sessao: o frontend trata logout como local-first
/// (`auth.store.ts:104-109`), e devolver erro para quem ja estava deslogado so' produziria um toast
/// sobre uma operacao que atingiu o resultado desejado.
/// </summary>
public class Logout : EndpointWithoutRequest<NoContent>
{
  public override void Configure()
  {
    Post(AuthRoutes.Logout);
    AllowAnonymous();
    Tags("Auth");
    Summary(s =>
    {
      s.Summary = "Encerra a sessão";
      s.Description = "Idempotente: 204 mesmo quando não havia sessão.";
    });
  }

  public override async Task<NoContent> ExecuteAsync(CancellationToken cancellationToken)
  {
    await HttpContext.SignOutSessionAsync();
    return TypedResults.NoContent();
  }
}
