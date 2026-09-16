using LicitaEdital.Application.Identity.Users.SetStatus;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;
using LicitaEdital.Facade.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Users;

/// <summary>
/// Reabilita o acesso de um usuario suspenso. Idempotente: reativar quem ja esta ativo responde
/// 200 com a mesma linha — o agregado nao registra evento nem grava nada nesse caso.
/// </summary>
public class Activate(IMediator mediator)
  : EndpointWithoutRequest<Results<Ok<UserResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(UsersRoutes.Activate);
    // `Policies(...)`, e **nao** `[RequireArea]`/`[RequirePermission]`: o FastEndpoints monta a
    // seguranca do endpoint a partir deste `Configure`, e ignora atributo de autorizacao na classe.
    // Com o atributo, este endpoint respondia **200 para um usuario de area `client` sem nenhuma
    // permissao** — autenticado, porem nao autorizado, e sem nenhum sinal de erro.
    //
    // Area e permissao, as duas: a area diz de qual metade do produto a sessao veio; a permissao diz
    // o que ela pode fazer ali dentro.
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Manager),
             AuthorizationPolicies.Permission(PermissionCode.Codes.UsersWrite));
    Tags("Users");
    Summary(s =>
    {
      s.Summary = "Reativa o acesso do usuário";
      s.Responses[200] = "Usuário ativo, no mesmo formato de GET /users/{id}";
      s.Responses[404] = "Não existe nesta organização";
    });
  }

  public override async Task<Results<Ok<UserResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CancellationToken cancellationToken)
  {
    // `EndpointWithoutRequest` + `Route<Guid>`, e nao um DTO de requisicao: o id vem da rota e nao
    // ha corpo. Com `Endpoint<TRequest>`, o FastEndpoints exige corpo JSON num POST e responde
    // **415** antes de chegar ao handler — o endpoint fica inalcancavel por um `POST` vazio, que e'
    // exatamente a forma desta operacao.
    var result = await _mediator.Send(
      new SetUserStatusCommand(UserId.From(Route<Guid>("id")), Active: true), cancellationToken);

    return result.ToUpdateResult(UserResponse.From);
  }
}
