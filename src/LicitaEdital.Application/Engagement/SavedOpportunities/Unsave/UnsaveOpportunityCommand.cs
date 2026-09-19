using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Application.Engagement.SavedOpportunities.Unsave;

public sealed record UnsaveOpportunityCommand(OpportunityId OpportunityId) : ICommand<Result>;
