using LicitaEdital.Domain.Offerings.OfferingAggregate.Events;

namespace LicitaEdital.Tasks.Catalog;

/// <summary>
/// A oferta mudou: toda compatibilidade calculada para aquela organizacao foi feita sobre o
/// vocabulario anterior.
///
/// <para>
/// <b>Recalcula, nao apenas invalida.</b> Apagar e esperar seria correto e mais barato, mas deixaria
/// o feed inteiro em `unrated` ate a proxima coleta — e quem acabou de cadastrar uma oferta espera
/// ver o efeito dela na tela seguinte, nao amanha. O custo e' assumido em
/// <see cref="CompatibilityRecalculator"/>, junto com a condicao para move-lo para uma fila.
/// </para>
/// </summary>
public class OfferingChangedHandler(CompatibilityRecalculator recalculator)
  : INotificationHandler<OfferingChangedEvent>
{
  private readonly CompatibilityRecalculator _recalculator = recalculator;

  public ValueTask Handle(OfferingChangedEvent notification, CancellationToken cancellationToken)
    => new(_recalculator.ForOrganizationAsync(notification.OrganizationId, cancellationToken));
}
