using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Core.Collections.CoverageSettingsAggregate;

[ValueObject<Guid>]
public readonly partial struct CoverageSettingsId : IGuidId<CoverageSettingsId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("CoverageSettingsId nao pode ser vazio.");
}
