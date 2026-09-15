using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.MembershipAggregate.Events;
using LicitaEdital.Facade.Identity;

namespace LicitaEdital.Tasks.Identity;

/// <summary>
/// Suspender um vinculo precisa derrubar a sessao viva — senao o cookie continua valendo ate
/// expirar, e `POST /users/{id}/deactivate` vira efeito cosmetico por ate oito horas.
///
/// <para>
/// Duas travas, e as duas sao necessarias: <c>GET /auth/me</c> ja rele o vinculo a cada carga da
/// aplicacao, o que cobre o boot; esta aqui invalida o carimbo de seguranca, o que derruba a
/// sessao em qualquer aba aberta, sem esperar recarga.
/// </para>
/// </summary>
public class MembershipStatusChangedHandler(
  IReadRepository<Membership> memberships,
  IUserAccountService accounts,
  ILogger<MembershipStatusChangedHandler> logger)
  : INotificationHandler<MembershipStatusChangedEvent>
{
  private readonly IReadRepository<Membership> _memberships = memberships;
  private readonly IUserAccountService _accounts = accounts;
  private readonly ILogger<MembershipStatusChangedHandler> _logger = logger;

  public async ValueTask Handle(MembershipStatusChangedEvent notification,
    CancellationToken cancellationToken)
  {
    if (notification.Status != MembershipStatus.Inactive) return;

    var membership = await _memberships.GetByIdAsync(notification.MembershipId, cancellationToken);
    if (membership is null) return;

    await _accounts.InvalidateSessionsAsync(membership.UserId, cancellationToken);

    _logger.LogInformation("Vinculo {MembershipId} suspenso: sessoes do usuario invalidadas",
      notification.MembershipId.Value);
  }
}
