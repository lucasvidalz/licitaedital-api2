using Vogen;

namespace LicitaEdital.Core.Identity.RoleAggregate;

[ValueObject<Guid>]
public readonly partial struct RoleId
{
  public static RoleId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("RoleId nao pode ser vazio.");
}
