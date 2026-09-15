using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Domain.Catalog.CompatibilityAggregate;

[ValueObject<Guid>]
public readonly partial struct OpportunityCompatibilityId : IGuidId<OpportunityCompatibilityId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("OpportunityCompatibilityId nao pode ser vazio.");
}
