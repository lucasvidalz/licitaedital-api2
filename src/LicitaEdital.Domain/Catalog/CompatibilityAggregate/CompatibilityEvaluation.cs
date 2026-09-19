using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Catalog.CompatibilityAggregate;

/// <summary>
/// O veredito do motor para o par (licitacao, oferta). Vira uma linha de
/// <see cref="OpportunityCompatibility"/>, e dali os campos de `compatibility` do contrato.
///
/// <para>
/// <b>As razoes sao parte do produto, nao enfeite.</b> Uma nota sozinha nao e' acionavel: quem le o
/// feed precisa saber **por que** aquela licitacao subiu, e o que conferir antes de investir tempo
/// nela. Por isso <see cref="PositiveReasons"/> e <see cref="AttentionPoints"/> saem do mesmo
/// calculo que a nota, e nao de uma prosa montada depois.
/// </para>
/// </summary>
public sealed record CompatibilityEvaluation(
  OfferingId OfferingId,
  string OfferingName,
  CompatibilityScore Score,
  IReadOnlyList<string> MatchedTerms,
  IReadOnlyList<string> PositiveReasons,
  IReadOnlyList<string> AttentionPoints);
