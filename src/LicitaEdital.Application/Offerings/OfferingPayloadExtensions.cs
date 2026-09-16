using Ardalis.SmartEnum;
using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.Domain.Offerings.OfferingAggregate;

namespace LicitaEdital.Application.Offerings;

/// <summary>
/// Traduz o corpo do contrato para o agregado. Um lugar so', usado por criacao e atualizacao —
/// `PUT /offerings/{id}` substitui a oferta inteira, entao as duas operacoes escrevem exatamente os
/// mesmos campos, e duplicar isso e' como elas divergem.
/// </summary>
public static class OfferingPayloadExtensions
{
  public static OfferingName Name(this OfferingPayload payload) => OfferingName.From(payload.Name);

  /// <summary>
  /// Aplica o corpo sobre a oferta. A ordem nao importa para o resultado; importa que **os tres**
  /// sejam chamados, porque o contrato nao tem campo parcial: lista ausente do corpo significa lista
  /// vazia, nao "mantenha o que estava".
  /// </summary>
  public static Offering ApplyTo(this OfferingPayload payload, Offering offering)
  {
    offering.Update(payload.Name(), payload.Description,
      SmartEnum<SupplyType, string>.FromValue(payload.SupplyType));

    offering.SetTerms(payload.PositiveKeywords, payload.NegativeKeywords, payload.Synonyms,
      payload.CatalogCodes);

    offering.SetCoverage(payload.ServedRegions.Select(StateCode.Parse), payload.MinValueCents,
      payload.MaxValueCents);

    return offering;
  }
}
