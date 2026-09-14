using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Core.Identity.RoleAggregate;

[ValueObject<Guid>]
public readonly partial struct RoleId : IGuidId<RoleId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("RoleId nao pode ser vazio.");
}
