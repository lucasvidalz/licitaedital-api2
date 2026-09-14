using Vogen;

namespace LicitaEdital.Core.Engagement.SavedOpportunityAggregate;

[ValueObject<Guid>]
public readonly partial struct SavedOpportunityId
{
  public static SavedOpportunityId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("SavedOpportunityId nao pode ser vazio.");
}
