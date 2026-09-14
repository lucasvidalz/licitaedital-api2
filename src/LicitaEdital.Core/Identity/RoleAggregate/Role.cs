using LicitaEdital.Core.Identity.MembershipAggregate;

namespace LicitaEdital.Core.Identity.RoleAggregate;

/// <summary>
/// Agrupa permissoes. E' de plataforma, nao por organizacao: os papeis sao os mesmos para todo
/// cliente, e permitir papel proprio por tenant abriria escalada de privilegio sem ganho de produto.
///
/// <see cref="Area"/> existe para impedir a combinacao errada — papel de gerenciamento nunca pode
/// ser atribuido a vinculo `client`, que e' o que a spec §16 exige ao dizer que gerenciador nao
/// ganha acesso a cofre, proposta nem poder de aprovacao de cliente.
/// </summary>
public class Role : AggregateRoot<RoleId>
{
  private readonly List<PermissionCode> _permissions = [];

  private Role(RoleName name, UserArea area)
  {
    Name = name;
    Area = area;
  }

  public RoleName Name { get; private set; }
  public UserArea Area { get; private set; }

  public IReadOnlyCollection<PermissionCode> Permissions => _permissions.AsReadOnly();

  public static Role Create(RoleName name, UserArea area, IEnumerable<PermissionCode> permissions)
  {
    var role = new Role(name, area);
    foreach (var permission in permissions)
    {
      role.Grant(permission);
    }
    return role;
  }

  public Role Grant(PermissionCode permission)
  {
    if (_permissions.Contains(permission)) return this;
    _permissions.Add(permission);
    return this;
  }

  public Role Revoke(PermissionCode permission)
  {
    _permissions.Remove(permission);
    return this;
  }

  public bool Allows(PermissionCode permission) => _permissions.Contains(permission);
}
