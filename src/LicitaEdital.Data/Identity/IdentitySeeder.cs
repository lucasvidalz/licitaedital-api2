using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.OrganizationAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;

namespace LicitaEdital.Data.Identity;

/// <summary>
/// Semente do modulo de identidade: a organizacao da plataforma e um papel padrao por area.
///
/// <para>
/// **E' semente de configuracao, nao de dado de runtime.** A regra 4 da constituicao proibe dado de
/// runtime em git; isto nao e' dado de cliente, e' a estrutura minima sem a qual o registro nao tem
/// papel para atribuir. Idempotente: roda a cada inicializacao e nao duplica.
/// </para>
///
/// <para>
/// O papel de cliente nasce **sem nenhuma permissao administrativa**. As permissoes que existem
/// hoje (`users.*`, `collections.read`, `participation-operations.*`) sao todas de gerenciamento —
/// a area do cliente e' autorizada pela propria area, nao por permissao nomeada. Conceder por
/// engano aqui seria escalada silenciosa.
/// </para>
/// </summary>
public static class IdentitySeeder
{
  public const string PlatformOrganizationName = "LicitaEdital";
  public const string ClientRoleName = "Cliente";
  public const string ManagerRoleName = "Gerenciador";

  public static async Task SeedAsync(IdentityDbContext context, CancellationToken cancellationToken = default)
  {
    var changed = false;

    if (!await context.Organizations.AnyAsync(o => o.IsPlatform, cancellationToken))
    {
      context.Organizations.Add(Organization.ForPlatform(OrganizationName.From(PlatformOrganizationName)));
      changed = true;
    }

    if (!await context.Roles.AnyAsync(role => role.Area == UserArea.Client, cancellationToken))
    {
      context.Roles.Add(Role.Create(RoleName.From(ClientRoleName), UserArea.Client, []));
      changed = true;
    }

    if (!await context.Roles.AnyAsync(role => role.Area == UserArea.Manager, cancellationToken))
    {
      // Gerenciador recebe as permissoes administrativas que as telas de /gfe exigem hoje. Nada de
      // participacao de cliente: a spec §16 e' explicita que gerenciador nao ganha cofre, proposta
      // nem poder de aprovacao.
      context.Roles.Add(Role.Create(RoleName.From(ManagerRoleName), UserArea.Manager,
      [
        PermissionCode.UsersRead,
        PermissionCode.UsersWrite,
        PermissionCode.CollectionsRead,
        PermissionCode.ParticipationOperationsRead,
        PermissionCode.ParticipationPortalsManage
      ]));
      changed = true;
    }

    if (changed) await context.SaveChangesAsync(cancellationToken);
  }
}
