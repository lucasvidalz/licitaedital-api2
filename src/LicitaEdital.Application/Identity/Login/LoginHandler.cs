using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.MembershipAggregate.Specifications;
using LicitaEdital.Facade.Identity;
using LicitaEdital.Facade.Shared;
using Microsoft.EntityFrameworkCore;

namespace LicitaEdital.Application.Identity.Login;

public class LoginHandler(
  IUserAccountService accounts,
  IAuthenticatedUserReader reader,
  IRepository<Membership> memberships,
  TimeProvider clock)
  : ICommandHandler<LoginCommand, Result<AuthenticatedUserDto>>
{
  private readonly IUserAccountService _accounts = accounts;
  private readonly IAuthenticatedUserReader _reader = reader;
  private readonly IRepository<Membership> _memberships = memberships;
  private readonly TimeProvider _clock = clock;

  public async ValueTask<Result<AuthenticatedUserDto>> Handle(LoginCommand command,
    CancellationToken cancellationToken)
  {
    var account = await _accounts.FindByEmailAsync(command.Email, cancellationToken);

    // **Uma unica resposta para todos os caminhos de falha**: e-mail inexistente, senha errada,
    // conta bloqueada e vinculo suspenso saem iguais. Diferenciar transformaria a tela de login num
    // verificador de quais e-mails existem na base.
    if (account is null) return InvalidCredentials();

    if (!await _accounts.CheckPasswordAsync(account.Id, command.Password, cancellationToken))
    {
      return InvalidCredentials();
    }

    var user = await _reader.ReadAsync(account.Id, cancellationToken);
    if (user is null) return InvalidCredentials();

    await _accounts.RegisterSuccessfulLoginAsync(account.Id, cancellationToken);
    await StampLastLoginAsync(OrganizationId.From(user.TenantId), account.Id, cancellationToken);

    return user;
  }

  /// <summary>
  /// Carimba o acesso no vinculo, que e' de onde `GET /users/{id}` le o `lastLoginAt`.
  ///
  /// <para>
  /// Fica no vinculo, e nao na conta do Identity, porque a tela mostra o ultimo acesso **dentro de
  /// uma organizacao** — o mesmo usuario em duas organizacoes tem duas respostas.
  /// </para>
  /// </summary>
  private async Task StampLastLoginAsync(OrganizationId organizationId, UserId userId,
    CancellationToken cancellationToken)
  {
    var membership = await _memberships.FirstOrDefaultAsync(
      new MembershipByUserSpec(organizationId, userId), cancellationToken);

    if (membership is null) return;

    membership.RegisterLogin(_clock);

    try
    {
      await _memberships.UpdateAsync(membership, cancellationToken);
    }
    catch (DbUpdateConcurrencyException)
    {
      // Dois logins simultaneos do mesmo usuario disputam a mesma linha. O perdedor nao tem nada a
      // corrigir: o vencedor gravou o mesmo carimbo, com milissegundos de diferenca. Engolir **esta**
      // excecao, e so' ela, evita transformar contabilidade de acesso em falha de autenticacao.
    }
  }

  private static Result<AuthenticatedUserDto> InvalidCredentials()
    => Result<AuthenticatedUserDto>.Unauthorized();
}
