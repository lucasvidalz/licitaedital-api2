using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Health;

/// <summary>
/// Sinal de vida da API. E' o criterio de pronto da Fase 0 do plano: a aplicacao sobe, o pipeline
/// monta e o cookie de XSRF e' emitido.
///
/// <para>
/// <b>Anonimo de proposito</b>, e essa e' a excecao que confirma a regra: endpoint novo nasce
/// autenticado (`CLAUDE.md`), e abrir exige motivo escrito. O motivo aqui e' que probe de
/// orquestrador e de balanceador nao carrega sessao — se este exigisse login, o container seria
/// reiniciado em laco por estar "nao saudavel".
/// </para>
///
/// <para>
/// Nao consulta banco nem dependencia externa: responde "o processo esta de pe e roteando".
/// Prontidao com dependencia e' outro assunto, e ja vem do Aspire por `MapDefaultEndpoints`.
/// </para>
/// </summary>
public class Get : EndpointWithoutRequest<Ok<HealthResponse>>
{
  public override void Configure()
  {
    base.Get(HealthResponse.Route);
    RoutePrefixOverride(string.Empty); // fora do `/api`: ver MiddlewareConfig
    AllowAnonymous();
    Tags("Health");
    Summary(s =>
    {
      s.Summary = "Sinal de vida da API";
      s.Description = "Responde 200 enquanto o processo estiver de pé e roteando. Não toca o banco.";
    });
  }

  public override Task<Ok<HealthResponse>> ExecuteAsync(CancellationToken cancellationToken)
    => Task.FromResult(TypedResults.Ok(new HealthResponse("ok")));
}

public record HealthResponse(string Status)
{
  public const string Route = "/health";
}
