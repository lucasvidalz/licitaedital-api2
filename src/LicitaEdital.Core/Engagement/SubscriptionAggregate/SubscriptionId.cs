using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Core.Engagement.SubscriptionAggregate;

[ValueObject<Guid>]
public readonly partial struct SubscriptionId : IGuidId<SubscriptionId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("SubscriptionId nao pode ser vazio.");
}
