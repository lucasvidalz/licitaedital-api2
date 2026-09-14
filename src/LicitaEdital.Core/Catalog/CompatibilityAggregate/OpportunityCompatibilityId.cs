using Vogen;

namespace LicitaEdital.Core.Catalog.CompatibilityAggregate;

[ValueObject<Guid>]
public readonly partial struct OpportunityCompatibilityId
{
  public static OpportunityCompatibilityId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("OpportunityCompatibilityId nao pode ser vazio.");
}
