using System.Globalization;
using System.Text;

namespace LicitaEdital.Domain.Catalog.CompatibilityAggregate;

/// <summary>
/// Decide quanto uma licitacao casa com o que a organizacao vende (D-07).
///
/// <para>
/// <b>Funcao pura, sem estado e sem I/O.</b> Recebe dois retratos e devolve o veredito. E' o que
/// permite testar cada regra sem banco, e o que permite chamar o motor tanto na ingestao quanto num
/// recalculo em lote sem duplicar a logica.
/// </para>
///
/// <para>
/// <b>A nota e' uma soma de pontos ganhos</b>, nao uma subtracao de penalidades. A diferenca importa
/// na leitura: 60 significa "ganhou 60 dos 100 possiveis", e cada parcela tem um motivo escrito em
/// <see cref="CompatibilityEvaluation.PositiveReasons"/>. Uma formula por penalidade produziria a
/// mesma nota sem conseguir explicar de onde ela veio.
/// </para>
/// </summary>
public static class CompatibilityEngine
{
  /// <summary>
  /// Versao do motor, gravada em cada linha calculada. **Suba quando a formula mudar**: e' o que
  /// permite a <see cref="OpportunityCompatibility.IsStale"/> distinguir uma linha antiga de uma
  /// linha recente, e recalcular so' o que a mudanca afetou.
  /// </summary>
  public const string Version = "1.0.0";

  // --- Os pesos. Somam 100, e essa e' a unica invariante da formula. ---

  /// <summary>Cobertura de termos: quanto do vocabulario da oferta aparece na licitacao.</summary>
  private const int TermsWeight = 50;

  /// <summary>Codigo de catalogo batendo com item publicado. Sinal forte, porque e' exato.</summary>
  private const int CatalogWeight = 25;

  /// <summary>UF da licitacao dentro das regioes atendidas.</summary>
  private const int RegionWeight = 15;

  /// <summary>Valor estimado dentro da faixa que a empresa disputa.</summary>
  private const int ValueWeight = 10;

  /// <summary>
  /// Avalia todas as ofertas e devolve **a melhor**. <c>null</c> quando nenhuma se aplica — que e' o
  /// estado `unrated` do contrato, e nao nota zero: zero afirmaria "avaliei e nao serve", e o que
  /// aconteceu foi "nao ha o que avaliar".
  /// </summary>
  public static CompatibilityEvaluation? Evaluate(OpportunityProfile opportunity,
    IReadOnlyList<OfferingProfile> offerings)
  {
    CompatibilityEvaluation? best = null;

    foreach (var offering in offerings)
    {
      var evaluation = Evaluate(opportunity, offering);
      if (evaluation is null) continue;

      if (best is null || evaluation.Score.Value > best.Score.Value) best = evaluation;
    }

    return best;
  }

  /// <summary>
  /// Avalia uma oferta. <c>null</c> quando ela **nao se aplica** a esta licitacao, o que acontece em
  /// dois casos: um termo negativo apareceu, ou nenhum termo positivo apareceu.
  /// </summary>
  public static CompatibilityEvaluation? Evaluate(OpportunityProfile opportunity,
    OfferingProfile offering)
  {
    var haystack = Normalize(string.Join(' ',
      [opportunity.Title, opportunity.Object, .. opportunity.ItemDescriptions]));

    // Termo negativo elimina, mesmo com termo positivo presente. E' a regra mais forte do motor, e e'
    // deliberado que ela venha antes de qualquer pontuacao: quem cadastrou "usado" como termo
    // negativo esta dizendo que aquela licitacao nao interessa, e uma nota alta com ressalva no
    // rodape seria ignorada na pratica.
    if (offering.NegativeKeywords.Any(term => Contains(haystack, term))) return null;

    var matchedPositives = offering.PositiveKeywords.Where(term => Contains(haystack, term)).ToList();
    var matchedSynonyms = offering.Synonyms.Where(term => Contains(haystack, term)).ToList();
    var matchedCodes = offering.CatalogCodes
      .Where(code => opportunity.ItemCatalogCodes.Any(published => CodeEquals(published, code)))
      .ToList();

    // Nenhum sinal positivo: a oferta nao tem o que dizer sobre esta licitacao. Devolver nota baixa
    // encheria o feed de ruido com aparencia de analise.
    if (matchedPositives.Count == 0 && matchedSynonyms.Count == 0 && matchedCodes.Count == 0)
    {
      return null;
    }

    var positiveReasons = new List<string>();
    var attentionPoints = new List<string>();
    var score = 0;

    score += TermCoverage(offering, matchedPositives, matchedSynonyms, positiveReasons);
    score += CatalogMatch(offering, matchedCodes, positiveReasons, attentionPoints);
    score += Region(opportunity, offering, positiveReasons, attentionPoints);
    score += Value(opportunity, offering, positiveReasons, attentionPoints);

    return new CompatibilityEvaluation(
      offering.Id,
      offering.Name,
      CompatibilityScore.From(Math.Clamp(score, CompatibilityScore.Min, CompatibilityScore.Max)),
      [.. matchedPositives, .. matchedSynonyms, .. matchedCodes],
      positiveReasons,
      attentionPoints);
  }

  /// <summary>
  /// Cobertura do vocabulario. Sinonimo vale **metade** de um termo positivo: ele confirma o assunto,
  /// mas foi cadastrado justamente por ser a forma menos precisa de nomea-lo.
  /// </summary>
  private static int TermCoverage(OfferingProfile offering, List<string> positives,
    List<string> synonyms, List<string> reasons)
  {
    var declared = offering.PositiveKeywords.Count;
    if (declared == 0)
    {
      // Oferta sem termo positivo cadastrado: nao da para medir cobertura. Sinonimo e codigo ainda
      // pontuam, e a falta aparece como o que e' — cadastro incompleto, nao incompatibilidade.
      return synonyms.Count > 0 ? TermsWeight / 4 : 0;
    }

    var covered = Math.Min(declared, positives.Count + (synonyms.Count / 2.0));
    var earned = (int)Math.Round(TermsWeight * covered / declared);

    if (positives.Count > 0)
    {
      reasons.Add($"{positives.Count} de {declared} termos da oferta aparecem no objeto: " +
                  $"{string.Join(", ", positives)}.");
    }

    if (synonyms.Count > 0)
    {
      reasons.Add($"Sinônimos encontrados: {string.Join(", ", synonyms)}.");
    }

    return earned;
  }

  private static int CatalogMatch(OfferingProfile offering, List<string> matchedCodes,
    List<string> reasons, List<string> attention)
  {
    if (matchedCodes.Count > 0)
    {
      reasons.Add($"Código de catálogo em comum: {string.Join(", ", matchedCodes)}.");
      return CatalogWeight;
    }

    if (offering.CatalogCodes.Count > 0)
    {
      // A oferta declara codigo e nenhum bateu. Nao e' eliminatorio — nem toda fonte publica
      // CATMAT/CATSER por item —, mas e' exatamente o tipo de conferencia que vale fazer antes de
      // montar proposta.
      attention.Add("Nenhum código de catálogo da oferta consta nos itens publicados.");
    }

    return 0;
  }

  /// <summary>
  /// UF atendida. Lista vazia significa **sem restricao**, e por isso ganha os pontos: a empresa
  /// declarou que atende qualquer lugar, nao que nao atende nenhum.
  ///
  /// <para>
  /// Estar fora da regiao <b>nao elimina</b>, so' custa os pontos e vira ressalva: entrega remota,
  /// representante local e consorcio sao comuns, e esconder a licitacao decidiria pelo cliente uma
  /// coisa que so' ele sabe.
  /// </para>
  /// </summary>
  private static int Region(OpportunityProfile opportunity, OfferingProfile offering,
    List<string> reasons, List<string> attention)
  {
    if (offering.ServedRegions.Count == 0) return RegionWeight;

    if (offering.ServedRegions.Contains(opportunity.State))
    {
      reasons.Add($"UF da licitação ({opportunity.State.Value}) está entre as regiões atendidas.");
      return RegionWeight;
    }

    attention.Add($"Licitação em {opportunity.State.Value}, fora das regiões atendidas pela oferta.");
    return 0;
  }

  /// <summary>
  /// Faixa de valor. Licitacao **sem valor publicado ganha os pontos**, pela mesma razao que o filtro
  /// do feed nao a exclui: o orgao nao e' obrigado a publicar estimativa, e punir a ausencia
  /// esconderia oportunidade real por um dado que nunca existiu.
  /// </summary>
  private static int Value(OpportunityProfile opportunity, OfferingProfile offering,
    List<string> reasons, List<string> attention)
  {
    if (opportunity.EstimatedValueCents is not { } estimated)
    {
      attention.Add("Licitação sem valor estimado publicado.");
      return ValueWeight;
    }

    if (offering.MinValueCents is { } min && estimated < min)
    {
      attention.Add("Valor estimado abaixo do piso definido na oferta.");
      return 0;
    }

    if (offering.MaxValueCents is { } max && estimated > max)
    {
      attention.Add("Valor estimado acima do teto definido na oferta.");
      return 0;
    }

    if (offering.MinValueCents is not null || offering.MaxValueCents is not null)
    {
      reasons.Add("Valor estimado dentro da faixa que a empresa disputa.");
    }

    return ValueWeight;
  }

  /// <summary>
  /// Casamento por substring sobre o texto normalizado. **Nao e' busca semantica** e nao pretende
  /// ser: o cliente cadastra os termos, e um casamento literal e' o que ele consegue prever e
  /// corrigir. Sinonimo existe justamente para cobrir a variacao que o literal nao pega.
  /// </summary>
  private static bool Contains(string haystack, string term)
  {
    var needle = Normalize(term);
    return needle.Length > 0 && haystack.Contains(needle, StringComparison.Ordinal);
  }

  private static bool CodeEquals(string published, string declared)
    => string.Equals(published.Trim(), declared.Trim(), StringComparison.OrdinalIgnoreCase);

  /// <summary>
  /// Minusculas e **sem acento**. Sem isso, "aquisicao" nao casaria com "aquisição" — e o cliente
  /// digita das duas formas, enquanto a fonte publica de um jeito so'.
  /// </summary>
  private static string Normalize(string value)
  {
    var decomposed = value.Trim().ToLower(CultureInfo.GetCultureInfo("pt-BR")).Normalize(NormalizationForm.FormD);
    var builder = new StringBuilder(decomposed.Length);

    foreach (var character in decomposed)
    {
      if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
      {
        builder.Append(character);
      }
    }

    return builder.ToString().Normalize(NormalizationForm.FormC);
  }
}
