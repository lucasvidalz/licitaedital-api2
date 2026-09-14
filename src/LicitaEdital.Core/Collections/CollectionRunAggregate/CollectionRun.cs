namespace LicitaEdital.Core.Collections.CollectionRunAggregate;

/// <summary>
/// Uma execucao do coletor. **Sem `OrganizationId`, e de proposito**: a coleta e' da plataforma, nao
/// de um cliente — e' o painel `/gfe/collections`, atras da permissao `collections.read`. E' a
/// segunda e ultima excecao a D-02, junto de <see cref="LicitaEdital.Core.Catalog.OpportunityAggregate.Opportunity"/>.
///
/// `lastSuccessfulRunAt`, que o envelope da listagem carrega, e' **derivado** — o maior
/// <see cref="EndedAt"/> entre as execucoes com <see cref="CollectionRunResult.Success"/>. Nao
/// guarde num contador a parte: dois lugares para a mesma verdade divergem na primeira falha
/// parcial.
/// </summary>
public class CollectionRun : EntityBase<CollectionRun, CollectionRunId>, IAggregateRoot
{
  private CollectionRun(DateTimeOffset startedAt)
  {
    StartedAt = startedAt;
    Result = CollectionRunResult.Partial;
  }

  public DateTimeOffset StartedAt { get; private set; }

  /// <summary>Nulo enquanto a execucao esta em andamento.</summary>
  public DateTimeOffset? EndedAt { get; private set; }

  public int NewOpportunitiesCount { get; private set; }
  public CollectionRunResult Result { get; private set; }

  /// <summary>Mensagem tecnica da falha. Nao expor detalhe interno ao cliente (SEC-64).</summary>
  public string? ErrorMessage { get; private set; }

  public static CollectionRun Start(TimeProvider clock)
      => new(clock.GetUtcNow()) { Id = CollectionRunId.New() };

  public CollectionRun Complete(CollectionRunResult result, int newOpportunitiesCount,
    string? errorMessage, TimeProvider clock)
  {
    Result = result;
    NewOpportunitiesCount = newOpportunitiesCount;
    ErrorMessage = errorMessage;
    EndedAt = clock.GetUtcNow();
    return this;
  }
}
