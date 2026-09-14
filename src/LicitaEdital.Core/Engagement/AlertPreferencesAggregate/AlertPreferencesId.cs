using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Core.Engagement.AlertPreferencesAggregate;

[ValueObject<Guid>]
public readonly partial struct AlertPreferencesId : IGuidId<AlertPreferencesId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("AlertPreferencesId nao pode ser vazio.");
}
