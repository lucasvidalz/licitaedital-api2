using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Application.Catalog.Opportunities.Details;

public sealed record GetOpportunityQuery(OpportunityId OpportunityId)
  : IQuery<Result<OpportunityDetailDto>>;
