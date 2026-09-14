using Vogen;

namespace LicitaEdital.Core.Engagement.SubscriptionAggregate;

[ValueObject<Guid>]
public readonly partial struct SubscriptionId
{
  public static SubscriptionId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("SubscriptionId nao pode ser vazio.");
}
