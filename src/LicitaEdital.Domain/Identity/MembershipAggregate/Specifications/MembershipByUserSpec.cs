using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Identity.MembershipAggregate.Specifications;

/// <summary>
/// O vinculo de um usuario **dentro de uma organizacao**.
///
/// <para>
/// A organizacao e' parametro obrigatorio, e nao um filtro opcional, de proposito: e' esta consulta
/// que `POST /users/{id}/activate|deactivate` usa para achar o alvo, e uma versao "por usuario" sem
/// organizacao deixaria um gerenciador suspender o vinculo de alguem de outro cliente. O indice
/// unico `ux_memberships_organization_user` garante no maximo uma linha ativa por par.
/// </para>
/// </summary>
public sealed class MembershipByUserSpec : SingleResultSpecification<Membership>
{
  public MembershipByUserSpec(OrganizationId organizationId, UserId userId)
  {
    Query.Where(membership => membership.OrganizationId == organizationId
                           && membership.UserId == userId);
  }
}
