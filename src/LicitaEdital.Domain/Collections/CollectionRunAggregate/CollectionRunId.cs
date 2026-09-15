using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Domain.Collections.CollectionRunAggregate;

[ValueObject<Guid>]
public readonly partial struct CollectionRunId : IGuidId<CollectionRunId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("CollectionRunId nao pode ser vazio.");
}
