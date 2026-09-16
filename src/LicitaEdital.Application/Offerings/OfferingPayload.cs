namespace LicitaEdital.Application.Offerings;

/// <summary>
/// O corpo de uma oferta, identico em criacao e atualizacao — `CreateOfferingRequest` do contrato e'
/// usado pelos dois (`offerings-api.service.ts`), porque `PUT /offerings/{id}` substitui a oferta
/// inteira e nao tem campo parcial.
///
/// <para>
/// As listas sao <c>IReadOnlyList</c> e nao anulaveis: lista vazia e ausencia significam a mesma
/// coisa aqui (<c>ServedRegions</c> vazio e' "sem restricao de UF"), e deixar o nulo entrar so'
/// espalharia <c>?? []</c> pelo handler.
/// </para>
/// </summary>
public sealed record OfferingPayload(
  string Name,
  string Description,
  IReadOnlyList<string> PositiveKeywords,
  IReadOnlyList<string> NegativeKeywords,
  IReadOnlyList<string> Synonyms,
  string SupplyType,
  IReadOnlyList<string> CatalogCodes,
  long? MinValueCents,
  long? MaxValueCents,
  IReadOnlyList<string> ServedRegions);
