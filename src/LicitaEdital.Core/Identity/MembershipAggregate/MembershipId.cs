using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Core.Identity.MembershipAggregate;

[ValueObject<Guid>]
public readonly partial struct MembershipId : IGuidId<MembershipId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("MembershipId nao pode ser vazio.");
}
