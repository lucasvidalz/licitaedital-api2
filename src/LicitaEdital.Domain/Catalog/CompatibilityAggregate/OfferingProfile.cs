using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Catalog.CompatibilityAggregate;

/// <summary>
/// O que o motor de compatibilidade precisa saber de uma oferta.
///
/// <para>
/// <b>E' um retrato, nao a entidade.</b> `Offering` vive no modulo Offerings, e Catalog nao alcanca
/// o schema de la (spec §4). Declarar aqui o recorte de que este modulo precisa e' o que permite o
/// motor ser uma funcao pura, sem dependencia de modulo nenhum — quem chama monta o retrato a partir
/// da fonte que tiver.
/// </para>
/// </summary>
public sealed record OfferingProfile(
  OfferingId Id,
  string Name,
  IReadOnlyList<string> PositiveKeywords,
  IReadOnlyList<string> NegativeKeywords,
  IReadOnlyList<string> Synonyms,
  IReadOnlyList<string> CatalogCodes,
  IReadOnlyList<StateCode> ServedRegions,
  long? MinValueCents,
  long? MaxValueCents);
