using Vogen;

namespace LicitaEdital.Core.Identity.MembershipAggregate;

[ValueObject<Guid>]
public readonly partial struct MembershipId
{
  public static MembershipId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("MembershipId nao pode ser vazio.");
}
