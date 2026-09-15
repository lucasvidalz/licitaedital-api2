namespace LicitaEdital.Domain.Catalog.OpportunityAggregate;

/// <summary>
/// Ordenacoes aceitas pelo feed. Os tres valores sao contrato — `?sort=` os recebe literalmente
/// (`opportunity.model.ts:36`).
///
/// <para>
/// SmartEnum, e nao <c>enum</c> nativo, mesmo nao sendo persistido: o valor **e** contrato com o
/// cliente, e <c>enum</c> nativo amarra esse contrato a uma constante numerica que ninguem declarou.
/// Com SmartEnum o texto que viaja e' explicito, <see cref="SmartEnum{TEnum,TValue}.FromValue"/>
/// valida a entrada na fronteira, e acrescentar uma ordenacao nao reordena nada por acidente.
/// </para>
/// </summary>
public sealed class OpportunitySort : SmartEnum<OpportunitySort, string>
{
  /// <summary>Compatibilidade decrescente. Ordenacao padrao do feed.</summary>
  public static readonly OpportunitySort Score = new(nameof(Score), "score");

  /// <summary>Prazo de proposta mais proximo primeiro. Sem prazo vai para o fim.</summary>
  public static readonly OpportunitySort Deadline = new(nameof(Deadline), "deadline");

  /// <summary>Publicacao mais recente primeiro.</summary>
  public static readonly OpportunitySort PublishedAt = new(nameof(PublishedAt), "publishedAt");

  private OpportunitySort(string name, string value) : base(name, value) { }

  /// <summary>
  /// Converte o texto do request. Valor desconhecido cai no padrao em vez de recusar a
  /// requisicao: ordenacao invalida na query string nao deveria derrubar o feed inteiro — a tela
  /// ainda tem o que mostrar, e o cliente ja limita as opcoes.
  /// </summary>
  public static OpportunitySort FromRequest(string? value)
    => TryFromValue(value ?? string.Empty, out var sort) ? sort : Score;
}
