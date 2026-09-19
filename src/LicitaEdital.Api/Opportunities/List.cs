using LicitaEdital.Application.Catalog.Opportunities.List;
using LicitaEdital.BuildingBlocks.Application.Paging;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Catalog.OpportunityAggregate;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Queries.Contracts.Catalog;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.Opportunities;

/// <summary>
/// O feed. Filtra por busca, UF, modalidade e faixa de valor; ordena por compatibilidade, prazo ou
/// publicacao; pagina.
///
/// <para>
/// A licitacao e' dado publico, mas o **score nao**: cada linha traz a compatibilidade da organizacao
/// da sessao. E' por isso que o endpoint e' autenticado apesar de listar dado publico.
/// </para>
/// </summary>
public class List(IMediator mediator)
  : Endpoint<ListOpportunitiesRequest,
             Results<Ok<PagedOpportunitiesResponse>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Get(OpportunitiesRoutes.List);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("Opportunities");
    Summary(s =>
    {
      s.Summary = "Feed de licitações";
      s.Description = "`state` e `modality` são multi-valor (`?state=SP&state=RJ`). " +
                      "`sort` aceita score, deadline e publishedAt; valor desconhecido cai em score.";
      s.Responses[200] = "Página do feed";
    });
  }

  public override async Task<Results<Ok<PagedOpportunitiesResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(ListOpportunitiesRequest request, CancellationToken cancellationToken)
  {
    var filter = new ListOpportunitiesFilter
    {
      Search = request.Search,
      States = [.. request.State],
      Modalities = [.. request.Modality],
      MinValueCents = request.MinValueCents,
      MaxValueCents = request.MaxValueCents
    };

    var result = await _mediator.Send(
      new ListOpportunitiesQuery(filter, OpportunitySort.FromRequest(request.Sort),
        new PageRequest(request.Page, request.PageSize)),
      cancellationToken);

    return result.ToGetResult(PagedOpportunitiesResponse.From);
  }
}
