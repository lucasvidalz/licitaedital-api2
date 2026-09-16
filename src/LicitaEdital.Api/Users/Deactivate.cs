using LicitaEdital.Application.Identity.Users.SetStatus;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;
using LicitaEdital.Facade.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Users;

/// <summary>
/// Suspende o acesso de um usuario.
///
/// <para>
/// **Derruba as sessoes abertas dele na hora**, em qualquer aba ou dispositivo. Sem isso o cookie
/// ja emitido continuaria valendo ate expirar, e "desativar" significaria "daqui a oito horas".
/// </para>
///
/// <para>
/// **Desativar a si mesmo responde 409.** E' a unica operacao desta tela que o gerenciador nao
/// consegue desfazer sozinho depois.
/// </para>
/// </summary>
public class Deactivate(IMediator mediator)
  : EndpointWithoutRequest<Results<Ok<UserResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Post(UsersRoutes.Deactivate);
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
      s.Summary = "Suspende o acesso do usuário";
      s.Responses[200] = "Usuário inativo, no mesmo formato de GET /users/{id}";
      s.Responses[404] = "Não existe nesta organização";
      s.Responses[409] = "Tentativa de desativar o próprio acesso";
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
      new SetUserStatusCommand(UserId.From(Route<Guid>("id")), Active: false), cancellationToken);

    return result.ToUpdateResult(UserResponse.From);
  }
}
