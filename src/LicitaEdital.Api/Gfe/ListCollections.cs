using LicitaEdital.Application.Collections.Runs.List;
using LicitaEdital.BuildingBlocks.Application.Paging;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Gfe;

/// <summary>
/// Historico de execucoes do coletor.
///
/// <para>
/// <b>Lista so' execucao concluida.</b> O contrato declara `endedAt` nao anulavel, e coleta em
/// andamento nao tem fim — inclui-la mandaria nulo num campo que a tela imprime direto. Coleta em
/// curso aparece quando termina.
/// </para>
/// </summary>
public class ListCollections(IMediator mediator)
  : Endpoint<ListCollectionRunsRequest,
             Results<Ok<CollectionRunsResponse>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(GfeRoutes.Collections);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Manager),
             AuthorizationPolicies.Permission(PermissionCode.Codes.CollectionsRead));
    Tags("Gfe");
    Summary(s =>
    {
      s.Summary = "Execuções do coletor";
      s.Description = "Paginado, mais recente primeiro, com `lastSuccessfulRunAt` no envelope.";
    });
  }

  public override async Task<Results<Ok<CollectionRunsResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(ListCollectionRunsRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new ListCollectionRunsQuery(new PageRequest(request.Page, request.PageSize)),
      cancellationToken);

    return result.ToGetResult(CollectionRunsResponse.From);
  }
}
