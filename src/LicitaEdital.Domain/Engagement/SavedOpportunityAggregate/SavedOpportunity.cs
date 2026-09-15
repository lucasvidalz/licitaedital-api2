using LicitaEdital.BuildingBlocks.Domain.Entities;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Engagement.SavedOpportunityAggregate;

/// <summary>
/// Marcacao de interesse da organizacao numa licitacao. Unica por (organizacao, licitacao) — salvar
/// duas vezes e' idempotente, nao gera segunda linha.
///
/// **Herda de <c>AuditableEntity</c>, nao de <c>AggregateRoot</c>, e isso e' deliberado: aqui a
/// exclusao e' fisica.** Dessalvar e' um alternador, nao a remocao de um registro de negocio; e com
/// soft delete a linha inativa continuaria ocupando o indice unico
/// (organizacao, licitacao), de modo que salvar de novo colidiria com a propria exclusao.
///
/// **O id do recurso na API e' o da licitacao, nao o desta entidade**:
/// `DELETE /saved-opportunities/{opportunityId}` (`saved-opportunities-api.service.ts:31`).
///
/// `savedAt` do contrato e' o <c>CreatedAt</c> da auditoria — nao ha campo proprio para a mesma
/// data.
///
/// Etapa, responsavel e prioridade do quadro de acompanhamento **nao entram aqui**: sao organizacao
/// local da equipe, guardadas no navegador (AD-042).
/// </summary>
public class SavedOpportunity : AuditableEntity<SavedOpportunityId>, IAggregateRoot, ITenantScoped
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

  Guid ITenantScoped.TenantId => OrganizationId.Value;

  public static SavedOpportunity Create(OrganizationId organizationId, OpportunityId opportunityId,
    UserId savedBy)
    => new(organizationId, opportunityId, savedBy);
}
