namespace LicitaEdital.Queries.Contracts.Catalog;

/// <summary>
/// Espelho de `OpportunityApiItem` (`opportunity-api.model.ts`). Os nomes **sao contrato**: o
/// frontend le campo a campo.
///
/// <c>Compatibility.Score</c> vai como numero; a faixa (`high`/`medium`/`low`/`poor`/`unrated`) e'
/// derivada no cliente (`opportunity.model.ts:1-7`) e **nao** deve ser calculada aqui — duas fontes
/// para a mesma classificacao divergem no primeiro ajuste de limiar.
/// </summary>
public sealed record OpportunityListItemDto(
  Guid Id,
  string Title,
  string Object,
  string BuyerName,
  string State,
  string City,
  string? CityIbgeCode,
  ModalityDto Modality,
  string Status,
  long? EstimatedValueCents,
  DateTimeOffset PublishedAt,
  DateTimeOffset? ProposalDeadline,
  string OfficialUrl,
  string Source,
  DateTimeOffset CollectedAt,
  CompatibilityDto Compatibility);

public sealed record ModalityDto(string Code, string Label);

public sealed record CompatibilityDto(
  int? Score,
  Guid? OfferingId,
  IReadOnlyList<string> MatchedTerms,
  IReadOnlyList<string> PositiveReasons,
  IReadOnlyList<string> AttentionPoints);
