using LicitaEdital.Domain.Catalog.OpportunityAggregate.Events;

namespace LicitaEdital.Tasks.Catalog;

/// <summary>
/// A coleta trouxe uma versao nova da licitacao: o score foi calculado sobre o objeto anterior.
///
/// Recalcula **para todas as organizacoes com oferta**, nao para uma: o objeto que mudou e' o
/// publico, e ele entra no calculo de todo mundo.
/// </summary>
public class OpportunityRefreshedHandler(CompatibilityRecalculator recalculator)
  : INotificationHandler<OpportunityRefreshedEvent>
{
  private readonly CompatibilityRecalculator _recalculator = recalculator;

  public ValueTask Handle(OpportunityRefreshedEvent notification, CancellationToken cancellationToken)
    => new(_recalculator.ForOpportunityAsync(notification.OpportunityId, cancellationToken));
}
