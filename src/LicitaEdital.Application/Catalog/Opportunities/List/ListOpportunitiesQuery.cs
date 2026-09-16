using LicitaEdital.Domain.Catalog.OpportunityAggregate;

namespace LicitaEdital.Application.Catalog.Opportunities.List;

/// <summary>Feed de oportunidades da organizacao da sessao.</summary>
public sealed record ListOpportunitiesQuery(
  ListOpportunitiesFilter Filter,
  OpportunitySort Sort,
  PageRequest Page) : IQuery<Result<PagedList<OpportunityListItemDto>>>;
