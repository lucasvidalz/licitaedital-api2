namespace LicitaEdital.Queries.Contracts.Catalog;

/// <summary>
/// Recorte do feed. Todos os campos sao opcionais — o feed sem filtro nenhum e' a primeira tela que
/// o cliente ve.
///
/// <c>States</c> e <c>Modalities</c> sao multi-valor porque o cliente envia
/// `?state=SP&amp;state=RJ` (`build-opportunities-query.helper.ts:38-46`). <c>Modalities</c> compara
/// o **codigo** da modalidade, nunca o rotulo (AD-030).
///
/// A ordenacao nao esta aqui: ela e' <c>OpportunitySort</c>, no dominio, junto do agregado que
/// ela ordena.
/// </summary>
public sealed record ListOpportunitiesFilter
{
  public string? Search { get; init; }
  public IReadOnlyCollection<string> States { get; init; } = [];
  public IReadOnlyCollection<string> Modalities { get; init; } = [];
  public long? MinValueCents { get; init; }
  public long? MaxValueCents { get; init; }
}
