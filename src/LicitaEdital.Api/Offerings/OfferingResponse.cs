using LicitaEdital.Queries.Contracts.Offerings;

namespace LicitaEdital.Api.Offerings;

/// <summary>
/// `OfferingApiItem` do contrato (`private/cfe/offerings/models/offering-api.model.ts`).
///
/// As cinco listas saem sempre presentes, possivelmente vazias — nunca ausentes. O mapper do
/// frontend se defende com `?? []`, mas essa defesa e' contra um corpo malformado, nao um convite
/// para produzir um.
/// </summary>
public sealed record OfferingResponse(
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
  DateTimeOffset CreatedAt)
{
  public static OfferingResponse From(OfferingDto offering) => new(
    offering.Id,
    offering.Name,
    offering.Description,
    offering.PositiveKeywords,
    offering.NegativeKeywords,
    offering.Synonyms,
    offering.SupplyType,
    offering.CatalogCodes,
    offering.MinValueCents,
    offering.MaxValueCents,
    offering.ServedRegions,
    offering.CreatedAt);
}
