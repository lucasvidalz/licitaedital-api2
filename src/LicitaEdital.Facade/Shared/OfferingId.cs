using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Facade.Shared;

/// <summary>
/// Identidade da oferta do cliente. Em Shared pelo mesmo motivo de
/// <see cref="OpportunityId"/>: Offerings a possui, e a projecao de compatibilidade em Catalog
/// registra qual oferta gerou o score.
/// </summary>
[ValueObject<Guid>]
public readonly partial struct OfferingId : IGuidId<OfferingId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("OfferingId nao pode ser vazio.");
}
