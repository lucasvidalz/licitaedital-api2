using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Offerings.OfferingAggregate.Events;

/// <summary>
/// A oferta mudou: toda compatibilidade ja calculada para esta organizacao ficou obsoleta.
/// Quem escuta esta em Catalog, dono da projecao — Offerings nao alcanca o schema de la.
/// </summary>
public class OfferingChangedEvent(OfferingId offeringId, OrganizationId organizationId) : DomainEventBase
{
  public OfferingId OfferingId { get; } = offeringId;
  public OrganizationId OrganizationId { get; } = organizationId;
}
