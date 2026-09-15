using LicitaEdital.BuildingBlocks.Domain.Entities;
using LicitaEdital.Domain.Identity.MembershipAggregate.Events;
using LicitaEdital.Domain.Identity.RoleAggregate;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Identity.MembershipAggregate;

/// <summary>
/// Vinculo entre um usuario e uma organizacao. E' aqui que moram area, papel e situacao — nao no
/// registro de identidade —, porque um mesmo usuario pode pertencer a mais de uma organizacao e a
/// resposta de `GET /auth/me` depende de qual esta ativa na sessao.
///
/// **<see cref="Status"/> e <c>IsActive</c> nao sao a mesma coisa, e a distincao importa.**
/// <see cref="Status"/> e' conceito de produto: a conta esta habilitada ou suspensa, e o
/// gerenciador alterna por `POST /users/{id}/activate|deactivate`. <c>IsActive</c>, herdado de
/// <c>AggregateRoot</c>, e' exclusao logica da linha: o vinculo foi removido da organizacao. Conta
/// suspensa continua na lista de usuarios; vinculo excluido some dela.
/// </summary>
public class Membership : AggregateRoot<MembershipId>, ITenantScoped
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

  /// <summary>
  /// Ultimo login bem-sucedido, exposto por `GET /users/{id}` (`user.model.ts:12`). Fica no vinculo,
  /// e nao no usuario, porque a tela de gerenciamento lista o acesso dentro de uma organizacao.
  /// </summary>
  public DateTimeOffset? LastLoginAt { get; private set; }

  Guid ITenantScoped.TenantId => OrganizationId.Value;

  public static Membership Create(OrganizationId organizationId, UserId userId, UserArea area,
    RoleId roleId)
    => new(organizationId, userId, area, roleId);

  public Membership Activate()
  {
    if (Status == MembershipStatus.Active) return this;
    Status = MembershipStatus.Active;
    RegisterDomainEvent(new MembershipStatusChangedEvent(Id, MembershipStatus.Active));
    return this;
  }

  public Membership Deactivate()
  {
    if (Status == MembershipStatus.Inactive) return this;
    Status = MembershipStatus.Inactive;
    RegisterDomainEvent(new MembershipStatusChangedEvent(Id, MembershipStatus.Inactive));
    return this;
  }

  public Membership AssignRole(RoleId roleId)
  {
    RoleId = roleId;
    return this;
  }

  public Membership RegisterLogin(TimeProvider clock)
  {
    LastLoginAt = clock.GetUtcNow();
    return this;
  }
}
