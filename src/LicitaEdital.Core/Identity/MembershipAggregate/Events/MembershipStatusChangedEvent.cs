namespace LicitaEdital.Core.Identity.MembershipAggregate.Events;

/// <summary>
/// Ativar ou desativar um vinculo precisa derrubar a sessao viva do usuario — senao o cookie
/// continua valendo ate expirar, e `POST /users/{id}/deactivate` vira efeito cosmetico.
/// </summary>
public class MembershipStatusChangedEvent(MembershipId membershipId, MembershipStatus status) : DomainEventBase
{
  public MembershipId MembershipId { get; } = membershipId;
  public MembershipStatus Status { get; } = status;
}
