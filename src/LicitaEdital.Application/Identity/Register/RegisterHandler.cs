using LicitaEdital.Facade.Identity;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.OrganizationAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate.Specifications;

namespace LicitaEdital.Queries.Contracts.Identity.Register;

/// <summary>
/// Cria conta, organizacao e vinculo, e **autentica na hora** — sem exigir confirmacao de e-mail.
/// Isso e' decisao de produto (`FEAT-12.2`, `AUTH-04`), nao descuido: o cadastro entrega valor na
/// primeira sessao, e a confirmacao vira um lembrete, nao um portao.
///
/// <para>
/// Cada registro abre uma **organizacao nova**: quem se cadastra pela tela publica e' uma empresa
/// chegando, nao alguem entrando numa organizacao existente. Convite para organizacao ja existente
/// e' outro fluxo, e nao tem contrato hoje.
/// </para>
/// </summary>
public class RegisterHandler(
  IUserAccountService accounts,
  IAuthenticationNotifier notifier,
  IRepository<Organization> organizations,
  IRepository<Membership> memberships,
  IReadRepository<Role> roles,
  IAuthenticatedUserReader reader)
  : ICommandHandler<RegisterCommand, Result<AuthenticatedUserDto>>
{
  private readonly IUserAccountService _accounts = accounts;
  private readonly IAuthenticationNotifier _notifier = notifier;
  private readonly IRepository<Organization> _organizations = organizations;
  private readonly IRepository<Membership> _memberships = memberships;
  private readonly IReadRepository<Role> _roles = roles;
  private readonly IAuthenticatedUserReader _reader = reader;

  public async ValueTask<Result<AuthenticatedUserDto>> Handle(RegisterCommand command,
    CancellationToken cancellationToken)
  {
    var clientRole = await _roles.FirstOrDefaultAsync(new DefaultRoleByAreaSpec(UserArea.Client),
      cancellationToken);

    if (clientRole is null)
    {
      // Papel padrao ausente e' falha de instalacao, nao erro do usuario: a semente nao rodou.
      throw new InvalidOperationException(
        "Papel padrao da area 'client' nao encontrado. A semente de identidade nao foi aplicada.");
    }

    var created = await _accounts.CreateAsync(command.Email, command.DisplayName, command.Password,
      cancellationToken);

    if (!created.IsSuccess) return Result<AuthenticatedUserDto>.Invalid(created.ValidationErrors);

    var organization = Organization.ForClient(OrganizationName.From(command.DisplayName));
    await _organizations.AddAsync(organization, cancellationToken);

    var membership = Membership.Create(organization.Id, created.Value.Id, UserArea.Client,
      clientRole.Id);
    await _memberships.AddAsync(membership, cancellationToken);

    // Disparo-e-esquece: a confirmacao nao bloqueia o cadastro, entao esperar pelo SMTP so' faria o
    // usuario olhar para um botao girando por causa de um e-mail que ele vai ler depois.
    var token = await _accounts.GenerateEmailConfirmationTokenAsync(created.Value.Id, cancellationToken);
    await _notifier.SendEmailConfirmationAsync(created.Value.Email, created.Value.DisplayName, token,
      cancellationToken);

    var user = await _reader.ReadAsync(created.Value.Id, cancellationToken);

    return user is null
      ? throw new InvalidOperationException("Vinculo recem-criado nao pode ser lido de volta.")
      : user;
  }
}
