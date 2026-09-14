using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Engagement.SubscriptionAggregate;

/// <summary>
/// Plano vigente da organizacao. `GET /subscription` hoje devolve **so** `{ planId }` — nao
/// acrescente ciclo, preco ou data de renovacao ao contrato antes de a tela pedir (SEC-64).
///
/// A troca de plano **nao tem endpoint**: o card de plano avisa que a mudanca corre pelo time
/// comercial (AD-039 do frontend). Por isso <see cref="ChangePlan"/> e' operacao interna, sem rota
/// publica — expor um PUT aqui criaria fluxo que nao termina.
/// </summary>
public class Subscription : EntityBase<Subscription, SubscriptionId>, IAggregateRoot
{
  private Subscription(OrganizationId organizationId, PlanId planId)
  {
    OrganizationId = organizationId;
    PlanId = planId;
  }

  public OrganizationId OrganizationId { get; private set; }
  public PlanId PlanId { get; private set; }

  public DateTimeOffset StartedAt { get; private set; }
  public DateTimeOffset UpdatedAt { get; private set; }

  public static Subscription Start(OrganizationId organizationId, PlanId planId, TimeProvider clock)
  {
    var now = clock.GetUtcNow();
    return new Subscription(organizationId, planId) { Id = SubscriptionId.New(), StartedAt = now, UpdatedAt = now };
  }

  public Subscription ChangePlan(PlanId planId, TimeProvider clock)
  {
    if (PlanId == planId) return this;
    PlanId = planId;
    UpdatedAt = clock.GetUtcNow();
    return this;
  }
}
