using LicitaEdital.Core.Identity.MembershipAggregate.Events;
using LicitaEdital.Core.Identity.RoleAggregate;
using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Identity.MembershipAggregate;

/// <summary>
/// Vinculo entre um usuario e uma organizacao. E' aqui que moram area, papel e situacao — nao no
/// registro de identidade —, porque um mesmo usuario pode pertencer a mais de uma organizacao e a
/// resposta de `GET /auth/me` depende de qual esta ativa na sessao.
///
/// `AuthUser.permissions` (contrato) sai do <see cref="RoleId"/>, resolvido no servidor. Permissao
/// nunca chega pelo payload do cliente (SEC-08).
/// </summary>
public class Membership : EntityBase<Membership, MembershipId>, IAggregateRoot
{
  private Membership(OrganizationId organizationId, UserId userId, UserArea area, RoleId roleId)
  {
    OrganizationId = organizationId;
    UserId = userId;
    Area = area;
    RoleId = roleId;
    Status = MembershipStatus.Active;
  }

  public OrganizationId OrganizationId { get; private set; }

  /// <summary>Referencia opaca ao registro do ASP.NET Core Identity, na mesma base.</summary>
  public UserId UserId { get; private set; }

  public UserArea Area { get; private set; }
  public RoleId RoleId { get; private set; }
  public MembershipStatus Status { get; private set; }

  public DateTimeOffset JoinedAt { get; private set; }
  public DateTimeOffset UpdatedAt { get; private set; }

  /// <summary>
  /// Ultimo login bem-sucedido, exposto por `GET /users/{id}` (`user.model.ts:12`). Fica no
  /// vinculo, e nao no usuario, porque a tela de gerenciamento lista o acesso dentro de uma
  /// organizacao.
  /// </summary>
  public DateTimeOffset? LastLoginAt { get; private set; }

  public static Membership Create(OrganizationId organizationId, UserId userId, UserArea area,
    RoleId roleId, TimeProvider clock)
  {
    var now = clock.GetUtcNow();
    return new Membership(organizationId, userId, area, roleId) { Id = MembershipId.New(), JoinedAt = now, UpdatedAt = now };
  }

  public Membership Activate(TimeProvider clock)
  {
    if (Status == MembershipStatus.Active) return this;
    Status = MembershipStatus.Active;
    UpdatedAt = clock.GetUtcNow();
    RegisterDomainEvent(new MembershipStatusChangedEvent(Id, MembershipStatus.Active));
    return this;
  }

  public Membership Deactivate(TimeProvider clock)
  {
    if (Status == MembershipStatus.Inactive) return this;
    Status = MembershipStatus.Inactive;
    UpdatedAt = clock.GetUtcNow();
    RegisterDomainEvent(new MembershipStatusChangedEvent(Id, MembershipStatus.Inactive));
    return this;
  }

  public Membership AssignRole(RoleId roleId, TimeProvider clock)
  {
    if (RoleId == roleId) return this;
    RoleId = roleId;
    UpdatedAt = clock.GetUtcNow();
    return this;
  }

  public Membership RegisterLogin(TimeProvider clock)
  {
    LastLoginAt = clock.GetUtcNow();
    return this;
  }
}
