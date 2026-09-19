using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Application.Engagement.SavedOpportunities.Save;

public sealed record SaveOpportunityCommand(OpportunityId OpportunityId)
  : ICommand<Result<SavedOpportunityView>>;
