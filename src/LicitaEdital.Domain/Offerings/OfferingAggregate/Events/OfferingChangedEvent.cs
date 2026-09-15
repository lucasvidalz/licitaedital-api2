using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Offerings.OfferingAggregate.Events;

/// <summary>
/// A oferta mudou: toda compatibilidade ja calculada para esta organizacao ficou obsoleta. Quem
/// escuta esta em Catalog, dono da projecao — Offerings nao alcanca o schema de la.
/// </summary>
public sealed record OfferingChangedEvent(OfferingId OfferingId, OrganizationId OrganizationId)
  : DomainEvent;
