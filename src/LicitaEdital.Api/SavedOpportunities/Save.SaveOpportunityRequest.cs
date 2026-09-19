using FluentValidation;

namespace LicitaEdital.Api.SavedOpportunities;

public class SaveOpportunityRequest
{
  public Guid OpportunityId { get; set; }
}

public class SaveOpportunityValidator : Validator<SaveOpportunityRequest>
{
  public SaveOpportunityValidator()
  {
    RuleFor(request => request.OpportunityId).NotEmpty();
  }
}
