namespace LicitaEdital.Queries.Contracts.Catalog;

/// <summary>
/// Espelho de `OpportunityDetailApiItem` (`opportunity-detail-api.model.ts`).
///
/// <para>
/// **Nao e' a listagem com campos a mais.** Ele traz `contractNumber`, itens e documentos, que a
/// listagem deliberadamente nao carrega — trazer item em pagina de 20 linhas multiplicaria a consulta
/// por algo que a tela do feed nem mostra. E deixa de fora `status`, que so' a listagem exibe.
/// </para>
/// </summary>
public sealed record OpportunityDetailDto(
  Guid Id,
  string Title,
  string Object,
  string BuyerName,
  string ContractNumber,
  string State,
  string City,
  string? CityIbgeCode,
  ModalityDto Modality,
  long? EstimatedValueCents,
  DateTimeOffset PublishedAt,
  DateTimeOffset? ProposalDeadline,
  string OfficialUrl,
  string Source,
  DateTimeOffset CollectedAt,
  IReadOnlyList<OpportunityLineItemDto> Items,
  IReadOnlyList<OpportunityDocumentDto> Documents,
  OpportunityDetailCompatibilityDto Compatibility);

public sealed record OpportunityLineItemDto(
  int Number,
  string Description,
  decimal Quantity,
  string Unit,
  long? UnitValueCents,
  long? TotalValueCents,
  string? CatalogCode);

public sealed record OpportunityDocumentDto(
  string Kind,
  string Label,
  string Url,
  DateTimeOffset? PublishedAt);

/// <summary>
/// Igual a <see cref="CompatibilityDto"/> **mais `OfferingName`**. O detalhe nomeia a oferta que
/// produziu a nota; a listagem so' devolve o id, porque repetir o nome em 20 linhas nao ajuda a
/// decidir e o feed ja agrupa por outra coisa.
/// </summary>
public sealed record OpportunityDetailCompatibilityDto(
  int? Score,
  Guid? OfferingId,
  string? OfferingName,
  IReadOnlyList<string> MatchedTerms,
  IReadOnlyList<string> PositiveReasons,
  IReadOnlyList<string> AttentionPoints);
