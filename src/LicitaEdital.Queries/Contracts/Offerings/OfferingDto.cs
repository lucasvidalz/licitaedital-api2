namespace LicitaEdital.Queries.Contracts.Offerings;

/// <summary>
/// Uma oferta, como a tela a le. Espelha `OfferingApiItem`
/// (`private/cfe/offerings/models/offering-api.model.ts`) campo a campo.
///
/// <para>
/// As cinco listas saem **sempre presentes, possivelmente vazias** — nunca nulas. O mapper do
/// frontend ja se defende com `?? []`, mas essa defesa existe porque um corpo sem a chave produziria
/// `undefined` num campo tipado como array; o servidor nao deve ser a origem desse problema.
/// </para>
/// </summary>
public sealed record OfferingDto(
  Guid Id,
  string Name,
  string Description,
  IReadOnlyList<string> PositiveKeywords,
  IReadOnlyList<string> NegativeKeywords,
  IReadOnlyList<string> Synonyms,
  string SupplyType,
  IReadOnlyList<string> CatalogCodes,
  long? MinValueCents,
  long? MaxValueCents,
  IReadOnlyList<string> ServedRegions,
  DateTimeOffset CreatedAt);
