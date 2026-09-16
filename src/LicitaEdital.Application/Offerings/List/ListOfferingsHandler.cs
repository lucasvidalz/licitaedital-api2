using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Offerings;

namespace LicitaEdital.Application.Offerings.List;

public class ListOfferingsHandler(IExecutionContext execution, IOfferingsQueryService offerings)
  : IQueryHandler<ListOfferingsQuery, Result<IReadOnlyList<OfferingDto>>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IOfferingsQueryService _offerings = offerings;

  public async ValueTask<Result<IReadOnlyList<OfferingDto>>> Handle(ListOfferingsQuery query,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId)
    {
      return Result<IReadOnlyList<OfferingDto>>.Unauthorized();
    }

    return Result<IReadOnlyList<OfferingDto>>.Success(
      await _offerings.ListAsync(OrganizationId.From(tenantId), cancellationToken));
  }
}
