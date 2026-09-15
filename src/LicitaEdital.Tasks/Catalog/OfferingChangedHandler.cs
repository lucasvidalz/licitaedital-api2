using LicitaEdital.Domain.Catalog.CompatibilityAggregate;
using LicitaEdital.Domain.Offerings.OfferingAggregate.Events;

namespace LicitaEdital.Tasks.Catalog;

/// <summary>
/// A oferta mudou: toda compatibilidade ja calculada para aquela organizacao ficou obsoleta.
///
/// <para>
/// <b>Apaga as linhas em vez de marcar para recalcular.</b> Score errado e' pior que score ausente:
/// a tela trata a ausencia como `unrated`, que e' honesto, enquanto uma nota velha afirma uma
/// compatibilidade que ninguem mais calculou. `OpportunityCompatibility` foi desenhada como
/// projecao sem soft delete exatamente para isto — apagar e' a operacao certa, e a coleta seguinte
/// recria.
/// </para>
/// </summary>
public class OfferingChangedHandler(
  IRepository<OpportunityCompatibility> compatibilities,
  ILogger<OfferingChangedHandler> logger)
  : INotificationHandler<OfferingChangedEvent>
{
  private readonly IRepository<OpportunityCompatibility> _compatibilities = compatibilities;
  private readonly ILogger<OfferingChangedHandler> _logger = logger;

  public async ValueTask Handle(OfferingChangedEvent notification, CancellationToken cancellationToken)
  {
    var stale = await _compatibilities.ListAsync(
      new CompatibilityByOrganizationSpec(notification.OrganizationId), cancellationToken);

    if (stale.Count == 0) return;

    await _compatibilities.DeleteRangeAsync(stale, cancellationToken);

    _logger.LogInformation(
      "Oferta {OfferingId} mudou: {Count} compatibilidade(s) da organizacao invalidadas",
      notification.OfferingId.Value, stale.Count);
  }
}
