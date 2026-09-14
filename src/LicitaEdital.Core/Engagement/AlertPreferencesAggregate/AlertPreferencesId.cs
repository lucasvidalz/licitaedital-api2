using Vogen;

namespace LicitaEdital.Core.Engagement.AlertPreferencesAggregate;

[ValueObject<Guid>]
public readonly partial struct AlertPreferencesId
{
  public static AlertPreferencesId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("AlertPreferencesId nao pode ser vazio.");
}
