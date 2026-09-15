using LicitaEdital.Facade.Identity;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Queries.Contracts.Identity;

namespace LicitaEdital.Queries.Identity;

/// <summary>
/// Monta o `AuthUser` da sessao: vinculo ativo, area, papel e permissoes — numa ida ao banco.
/// </summary>
public class AuthenticatedUserReader(IdentityReadContext context, IUserAccountService accounts)
  : IAuthenticatedUserReader
{
  private readonly IdentityReadContext _context = context;
  private readonly IUserAccountService _accounts = accounts;

  public async Task<AuthenticatedUserDto?> ReadAsync(UserId userId,
    CancellationToken cancellationToken = default)
  {
    var session = await (
      from membership in _context.Memberships
      where membership.UserId == userId && membership.Status == MembershipStatus.Active
      join role in _context.Roles on membership.RoleId equals role.Id
      select new { membership.OrganizationId, membership.Area, Role = role })
      .FirstOrDefaultAsync(cancellationToken);

    // Sem vinculo ativo nao ha sessao. Conta suspensa cai aqui, e e' o que faz `deactivate` ter
    // efeito de verdade — o filtro de soft delete do contexto ja exclui vinculo removido.
    if (session is null) return null;

    // E-mail e nome vem do provedor de identidade, nao de uma copia nossa: duas fontes para o
    // mesmo dado divergem na primeira troca de e-mail.
    var account = await _accounts.FindByIdAsync(userId, cancellationToken);
    if (account is null) return null;

    return new AuthenticatedUserDto(
      userId.Value,
      account.Email,
      account.DisplayName,
      session.OrganizationId.Value,
      session.Area.Value,
      [.. session.Role.Permissions.Select(permission => permission.Value)]);
  }
}
