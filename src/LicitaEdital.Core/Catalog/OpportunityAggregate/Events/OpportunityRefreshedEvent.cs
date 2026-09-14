using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Catalog.OpportunityAggregate.Events;

/// <summary>
/// A coleta trouxe uma versao nova desta licitacao. Quem escuta recalcula a compatibilidade de
/// todas as organizacoes — o score foi calculado sobre o objeto anterior.
/// </summary>
public class OpportunityRefreshedEvent(OpportunityId opportunityId) : DomainEventBase
{
  public OpportunityId OpportunityId { get; } = opportunityId;
}
