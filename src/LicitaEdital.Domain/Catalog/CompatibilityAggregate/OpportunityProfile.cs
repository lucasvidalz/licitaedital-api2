using LicitaEdital.Domain.Catalog.OpportunityAggregate;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Catalog.CompatibilityAggregate;

/// <summary>
/// O que o motor precisa saber da licitacao. Retrato, como <see cref="OfferingProfile"/> — aqui nao
/// por fronteira de modulo, mas para que o motor seja testavel sem montar o agregado inteiro com
/// itens, documentos e datas.
/// </summary>
public sealed record OpportunityProfile(
  string Title,
  string Object,
  StateCode State,
  long? EstimatedValueCents,
  IReadOnlyList<string> ItemDescriptions,
  IReadOnlyList<string> ItemCatalogCodes)
{
  public static OpportunityProfile From(Opportunity opportunity) => new(
    opportunity.Title,
    opportunity.Object,
    opportunity.State,
    opportunity.EstimatedValueCents,
    [.. opportunity.Items.Select(item => item.Description)],
    [.. opportunity.Items.Select(item => item.CatalogCode).OfType<string>()]);
}
