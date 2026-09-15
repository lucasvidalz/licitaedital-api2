using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Catalog.OpportunityAggregate.Events;

/// <summary>
/// A coleta trouxe uma versao nova desta licitacao. Quem escuta recalcula a compatibilidade de todas
/// as organizacoes — o score foi calculado sobre o objeto anterior.
/// </summary>
public sealed record OpportunityRefreshedEvent(OpportunityId OpportunityId) : DomainEvent;
