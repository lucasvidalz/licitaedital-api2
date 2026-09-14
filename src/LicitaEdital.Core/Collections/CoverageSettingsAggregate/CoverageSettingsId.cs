using Vogen;

namespace LicitaEdital.Core.Collections.CoverageSettingsAggregate;

[ValueObject<Guid>]
public readonly partial struct CoverageSettingsId
{
  public static CoverageSettingsId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("CoverageSettingsId nao pode ser vazio.");
}
