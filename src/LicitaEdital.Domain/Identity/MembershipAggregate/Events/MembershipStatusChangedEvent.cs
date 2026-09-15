namespace LicitaEdital.Domain.Identity.MembershipAggregate.Events;

/// <summary>
/// Suspender ou reabilitar um vinculo precisa derrubar a sessao viva do usuario — senao o cookie
/// continua valendo ate expirar, e `POST /users/{id}/deactivate` vira efeito cosmetico.
/// </summary>
public sealed record MembershipStatusChangedEvent(MembershipId MembershipId, MembershipStatus Status)
  : DomainEvent;
