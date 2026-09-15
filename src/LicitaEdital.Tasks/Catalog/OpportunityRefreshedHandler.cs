using LicitaEdital.Domain.Catalog.CompatibilityAggregate;
using LicitaEdital.Domain.Catalog.OpportunityAggregate.Events;

namespace LicitaEdital.Tasks.Catalog;

/// <summary>
/// A coleta trouxe uma versao nova da licitacao: o score foi calculado sobre o objeto anterior.
///
/// Invalida **de todas as organizacoes**, nao de uma: o objeto que mudou e' o publico, e ele entra
/// no calculo de todo mundo.
/// </summary>
public class OpportunityRefreshedHandler(
  IRepository<OpportunityCompatibility> compatibilities,
  ILogger<OpportunityRefreshedHandler> logger)
  : INotificationHandler<OpportunityRefreshedEvent>
{
  private readonly IRepository<OpportunityCompatibility> _compatibilities = compatibilities;
  private readonly ILogger<OpportunityRefreshedHandler> _logger = logger;

  public async ValueTask Handle(OpportunityRefreshedEvent notification,
    CancellationToken cancellationToken)
  {
    var stale = await _compatibilities.ListAsync(
      new CompatibilityByOpportunitySpec(notification.OpportunityId), cancellationToken);

    if (stale.Count == 0) return;

    await _compatibilities.DeleteRangeAsync(stale, cancellationToken);

    _logger.LogInformation(
      "Licitacao {OpportunityId} atualizada: {Count} compatibilidade(s) invalidadas",
      notification.OpportunityId.Value, stale.Count);
  }
}
