using LicitaEdital.BuildingBlocks.Domain.Entities;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Engagement.SubscriptionAggregate;

/// <summary>
/// Plano vigente da organizacao. `GET /subscription` hoje devolve **so** `{ planId }` — nao
/// acrescente ciclo, preco ou data de renovacao ao contrato antes de a tela pedir (SEC-64).
///
/// A troca de plano **nao tem endpoint**: o card de plano avisa que a mudanca corre pelo time
/// comercial (AD-039 do frontend). Por isso <see cref="ChangePlan"/> e' operacao interna, sem rota
/// publica — expor um PUT aqui criaria fluxo que nao termina.
/// </summary>
public class Subscription : AggregateRoot<SubscriptionId>, ITenantScoped
{
  private Subscription(OrganizationId organizationId, PlanId planId, DateTimeOffset startedAt)
  {
    OrganizationId = organizationId;
    PlanId = planId;
    StartedAt = startedAt;
  }

  public OrganizationId OrganizationId { get; private set; }
  public PlanId PlanId { get; private set; }

  /// <summary>
  /// Inicio da vigencia. Campo proprio, e nao <c>CreatedAt</c>: uma assinatura pode ser registrada
  /// hoje com vigencia retroativa, e confundir as duas datas apagaria essa diferenca.
  /// </summary>
  public DateTimeOffset StartedAt { get; private set; }

  Guid ITenantScoped.TenantId => OrganizationId.Value;

  public static Subscription Start(OrganizationId organizationId, PlanId planId, TimeProvider clock)
      => new(organizationId, planId, clock.GetUtcNow());

  public static Subscription StartAt(OrganizationId organizationId, PlanId planId,
    DateTimeOffset startedAt)
    => new(organizationId, planId, startedAt);

  public Subscription ChangePlan(PlanId planId)
  {
    PlanId = planId;
    return this;
  }
}
