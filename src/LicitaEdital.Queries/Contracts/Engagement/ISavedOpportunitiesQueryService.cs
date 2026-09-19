namespace LicitaEdital.Queries.Contracts.Engagement;

public interface ISavedOpportunitiesQueryService
{
  /// <summary>
  /// As licitacoes salvas pela organizacao, **mais recente primeiro**. A ordem sai daqui e nao do
  /// chamador: `savedAt` e' o unico criterio da tela, e a fachada de Catalog devolve na ordem dela.
  /// </summary>
  Task<IReadOnlyList<SavedOpportunityRefDto>> ListAsync(OrganizationId organizationId,
    CancellationToken cancellationToken = default);

  /// <summary>Se a organizacao ja salvou aquela licitacao. Torna `POST` idempotente sem ir ao agregado.</summary>
  Task<bool> ExistsAsync(OrganizationId organizationId, OpportunityId opportunityId,
    CancellationToken cancellationToken = default);
}
