namespace LicitaEdital.Queries.Contracts.Engagement;

/// <summary>
/// Uma linha de salvamento: **so' a referencia e a data**.
///
/// <para>
/// A licitacao em si nao vem daqui, e nao poderia: ela mora no schema `catalog`, e Engagement nao o
/// alcanca (spec §4). Quem junta as duas metades e' o caso de uso, chamando
/// <c>ICatalogFacade</c> — duas consultas independentes, uma por modulo.
/// </para>
/// </summary>
public sealed record SavedOpportunityRefDto(Guid OpportunityId, DateTimeOffset SavedAt);
