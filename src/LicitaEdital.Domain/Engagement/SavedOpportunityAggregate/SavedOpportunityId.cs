using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Domain.Engagement.SavedOpportunityAggregate;

[ValueObject<Guid>]
public readonly partial struct SavedOpportunityId : IGuidId<SavedOpportunityId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("SavedOpportunityId nao pode ser vazio.");
}
