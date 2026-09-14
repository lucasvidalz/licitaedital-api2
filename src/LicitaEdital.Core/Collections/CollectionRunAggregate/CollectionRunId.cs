using Vogen;

namespace LicitaEdital.Core.Collections.CollectionRunAggregate;

[ValueObject<Guid>]
public readonly partial struct CollectionRunId
{
  public static CollectionRunId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("CollectionRunId nao pode ser vazio.");
}
