using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Engagement.SavedOpportunityAggregate;

/// <summary>
/// Marcacao de interesse da organizacao numa licitacao. Unica por (organizacao, licitacao) — salvar
/// duas vezes e' idempotente, nao gera segunda linha.
///
/// **O id do recurso na API e' o da licitacao, nao o desta entidade**:
/// `DELETE /saved-opportunities/{opportunityId}` (`saved-opportunities-api.service.ts:31`). O
/// <see cref="SavedOpportunityId"/> e' interno.
///
/// O que a API devolve embute a `OpportunityApiItem` inteira, montada pelo query service — esta
/// entidade guarda so a referencia opaca, sem navegacao para Catalog.
///
/// Etapa, responsavel e prioridade do quadro de acompanhamento **nao entram aqui**: sao organizacao
/// local da equipe, guardadas no navegador (AD-042 do frontend). Trazer para o servidor agora seria
/// inventar recurso que o contrato nao tem.
/// </summary>
public class SavedOpportunity : EntityBase<SavedOpportunity, SavedOpportunityId>, IAggregateRoot
{
  private SavedOpportunity(OrganizationId organizationId, OpportunityId opportunityId, UserId savedBy)
  {
    OrganizationId = organizationId;
    OpportunityId = opportunityId;
    SavedBy = savedBy;
  }

  public OrganizationId OrganizationId { get; private set; }
  public OpportunityId OpportunityId { get; private set; }

  /// <summary>Quem salvou. Nao aparece no contrato hoje; existe para trilha de auditoria.</summary>
  public UserId SavedBy { get; private set; }

  public DateTimeOffset SavedAt { get; private set; }

  public static SavedOpportunity Create(OrganizationId organizationId, OpportunityId opportunityId,
    UserId savedBy, TimeProvider clock)
    => new(organizationId, opportunityId, savedBy)
    {
      Id = SavedOpportunityId.New(),
      SavedAt = clock.GetUtcNow()
    };
}
