using LicitaEdital.Application.Identity.Users.Get;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;
using LicitaEdital.Facade.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Users;

/// <summary>
/// Detalhe de um usuario da organizacao.
///
/// Usuario de outra organizacao responde **404**, e nao 403: 403 confirmaria que aquele id existe
/// em algum lugar, o que ja e' informacao sobre a base de outro cliente (spec §16).
/// </summary>
public class Get(IMediator mediator)
  : Endpoint<UserByIdRequest, Results<Ok<UserResponse>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    base.Get(UsersRoutes.Get);
    // `Policies(...)`, e **nao** `[RequireArea]`/`[RequirePermission]`: o FastEndpoints monta a
    // seguranca do endpoint a partir deste `Configure`, e ignora atributo de autorizacao na classe.
    // Com o atributo, este endpoint respondia **200 para um usuario de area `client` sem nenhuma
    // permissao** — autenticado, porem nao autorizado, e sem nenhum sinal de erro.
    //
    // Area e permissao, as duas: a area diz de qual metade do produto a sessao veio; a permissao diz
    // o que ela pode fazer ali dentro.
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Manager),
             AuthorizationPolicies.Permission(PermissionCode.Codes.UsersRead));
    Tags("Users");
    Summary(s =>
    {
      s.Summary = "Detalhe de um usuário";
      s.Responses[200] = "Usuário encontrado";
      s.Responses[404] = "Não existe nesta organização";
    });
  }

  public override async Task<Results<Ok<UserResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
    UserByIdRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetUserQuery(UserId.From(request.Id)), cancellationToken);

    return result.ToGetResult(UserResponse.From);
  }
}
