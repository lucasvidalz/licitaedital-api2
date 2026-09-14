using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LicitaEdital.Infrastructure.Data;

/// <summary>
/// Despacha os eventos de dominio **depois** que a gravacao teve sucesso — nao antes: evento
/// publicado numa transacao que depois falha anuncia um fato que nao aconteceu.
///
/// Serve os seis contextos de modulo, e nao um em particular: o interceptor le o ChangeTracker do
/// contexto que gravou, qualquer que seja.
/// </summary>
public class EventDispatchInterceptor(IDomainEventDispatcher domainEventDispatcher) : SaveChangesInterceptor
{
  private readonly IDomainEventDispatcher _domainEventDispatcher = domainEventDispatcher;

  public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
    CancellationToken cancellationToken = new CancellationToken())
  {
    var context = eventData.Context;
    if (context is null)
    {
      return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    var entitiesWithEvents = context.ChangeTracker.Entries<HasDomainEventsBase>()
      .Select(entry => entry.Entity)
      .Where(entity => entity.DomainEvents.Any())
      .ToArray();

    await _domainEventDispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return await base.SavedChangesAsync(eventData, result, cancellationToken);
  }
}
