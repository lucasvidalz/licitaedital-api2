using LicitaEdital.Core.Identity.MembershipAggregate;

namespace LicitaEdital.Core.Identity.RoleAggregate.Specifications;

/// <summary>
/// Papel padrao de uma area. Ha exatamente um por area, criado pela semente de identidade — o
/// indice unico em `Name` nao garante isso, entao quem cria papel novo numa area precisa saber que
/// esta consulta pega o primeiro.
/// </summary>
public sealed class DefaultRoleByAreaSpec : Specification<Role>
{
  public DefaultRoleByAreaSpec(UserArea area)
  {
    Query.Where(role => role.Area == area);
  }
}
