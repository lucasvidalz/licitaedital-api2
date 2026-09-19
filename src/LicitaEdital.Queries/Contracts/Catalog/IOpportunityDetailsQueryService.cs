namespace LicitaEdital.Queries.Contracts.Catalog;

/// <summary>
/// Detalhe de uma licitacao, com a compatibilidade **daquela organizacao**.
///
/// <para>
/// <b>404 aqui e' erro de verdade</b>, ao contrario de `GET /company-profile`: a licitacao e' dado
/// publico e o id veio de um link que o proprio feed produziu. Ausencia significa id inventado ou
/// licitacao despublicada, e a tela mostra estado de erro (`AD-032` distingue os dois casos).
/// </para>
/// </summary>
public interface IOpportunityDetailsQueryService
{
  Task<OpportunityDetailDto?> FindAsync(OrganizationId organizationId, OpportunityId opportunityId,
    CancellationToken cancellationToken = default);
}
