namespace LicitaEdital.Application.Engagement.SavedOpportunities.List;

public sealed record ListSavedOpportunitiesQuery
  : IQuery<Result<IReadOnlyList<SavedOpportunityView>>>;
