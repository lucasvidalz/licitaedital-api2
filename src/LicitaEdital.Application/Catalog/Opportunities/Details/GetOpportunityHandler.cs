using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Application.Catalog.Opportunities.Details;

/// <summary>
/// A licitacao e' **publica** — nao ha tenant a conferir nela. A organizacao entra por outro motivo:
/// e' ela que decide **qual compatibilidade** acompanha a resposta. Duas empresas abrindo o mesmo
/// link veem a mesma licitacao com notas diferentes, e e' assim que deve ser.
/// </summary>
public class GetOpportunityHandler(
  IExecutionContext execution,
  IOpportunityDetailsQueryService opportunities)
  : IQueryHandler<GetOpportunityQuery, Result<OpportunityDetailDto>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IOpportunityDetailsQueryService _opportunities = opportunities;

  public async ValueTask<Result<OpportunityDetailDto>> Handle(GetOpportunityQuery query,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<OpportunityDetailDto>.Unauthorized();

    var opportunity = await _opportunities.FindAsync(OrganizationId.From(tenantId),
      query.OpportunityId, cancellationToken);

    return opportunity is null ? Result<OpportunityDetailDto>.NotFound() : opportunity;
  }
}
