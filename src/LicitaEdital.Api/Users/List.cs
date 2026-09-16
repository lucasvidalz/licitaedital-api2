using LicitaEdital.Application.Identity.Users.List;
using LicitaEdital.BuildingBlocks.Application.Paging;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Users;

/// <summary>
/// Lista paginada de usuarios da organizacao (`FEAT-09`).
///
/// <para>
/// <b>Area e permissao, as duas.</b> A area diz de qual metade do produto a sessao veio; a permissao
/// diz o que ela pode fazer ali dentro. Exigir so' a permissao deixaria um papel de cliente com
/// `users.read` — que hoje nao existe, mas que uma semente futura pode criar — ler a tela de
/// gerenciamento.
/// </para>
/// </summary>
public class List(IMediator mediator)
  : Endpoint<ListUsersRequest, Results<Ok<PagedUsersResponse>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(UsersRoutes.List);
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
      s.Summary = "Lista os usuários da organização";
      s.Description = "Pagina, busca por nome ou e-mail e filtra por situação.";
      s.Responses[200] = "Página de usuários";
      s.Responses[401] = "Sem sessão";
      s.Responses[403] = "Sessão sem a permissão users.read ou fora da área de gerenciamento";
    });
  }

  public override async Task<Results<Ok<PagedUsersResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
    ListUsersRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new ListUsersQuery(new PageRequest(request.Page, request.PageSize), request.Search,
        request.Status),
      cancellationToken);

    return result.ToGetResult(PagedUsersResponse.From);
  }
}
