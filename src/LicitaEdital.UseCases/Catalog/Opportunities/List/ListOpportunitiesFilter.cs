namespace LicitaEdital.UseCases.Catalog.Opportunities.List;

/// <summary>
/// Ordenacoes aceitas por `GET /opportunities?sort=`. Os tres valores sao contrato
/// (`opportunity.model.ts:36`), mas o **nome** aqui nao viaja: a traducao do texto do request para
/// este enum e' do endpoint. Por isso `enum` simples, e nao SmartEnum — nada disto e' persistido.
/// </summary>
public enum OpportunitySort
{
  /// <summary>Compatibilidade decrescente. Ordenacao padrao do feed.</summary>
  Score,

  /// <summary>Prazo de proposta mais proximo primeiro. Sem prazo vai para o fim.</summary>
  Deadline,

  /// <summary>Publicacao mais recente primeiro.</summary>
  PublishedAt
}

/// <summary>
/// Recorte do feed. Todos os campos sao opcionais — o feed sem filtro nenhum e' a primeira tela que
/// o cliente ve.
///
/// <c>States</c> e <c>Modalities</c> sao multi-valor porque o cliente envia
/// `?state=SP&amp;state=RJ` (`build-opportunities-query.helper.ts:38-46`). <c>Modalities</c> compara
/// o **codigo** da modalidade, nunca o rotulo (AD-030).
/// </summary>
public sealed record ListOpportunitiesFilter
{
  public string? Search { get; init; }
  public IReadOnlyCollection<string> States { get; init; } = [];
  public IReadOnlyCollection<string> Modalities { get; init; } = [];
  public long? MinValueCents { get; init; }
  public long? MaxValueCents { get; init; }
}
