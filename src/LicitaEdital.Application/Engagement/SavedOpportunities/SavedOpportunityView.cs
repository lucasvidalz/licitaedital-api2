using LicitaEdital.Facade.Catalog;

namespace LicitaEdital.Application.Engagement.SavedOpportunities;

/// <summary>
/// Uma licitacao salva, **com a licitacao inteira embutida** — e' o que o contrato manda
/// (`SavedOpportunityApiItem`), nao so' o id.
///
/// <para>
/// Montada na aplicacao, juntando duas fontes: a linha de salvamento vem de Engagement, a licitacao
/// vem de <c>ICatalogFacade</c>. Nao ha consulta unica possivel — os dois modulos tem schemas
/// separados, e um join entre eles e' o que a §4 proibe.
/// </para>
/// </summary>
public sealed record SavedOpportunityView(DateTimeOffset SavedAt, OpportunitySummary Opportunity);
