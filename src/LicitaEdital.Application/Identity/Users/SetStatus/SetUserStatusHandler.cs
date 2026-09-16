using LicitaEdital.BuildingBlocks.Application.Errors;
using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.MembershipAggregate.Specifications;
using LicitaEdital.Facade.Identity;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Application.Identity.Users.SetStatus;

/// <summary>
/// Habilita ou suspende o vinculo de um usuario na organizacao da sessao.
/// </summary>
public class SetUserStatusHandler(
  IExecutionContext execution,
  IRepository<Membership> memberships,
  IUserAccountService accounts,
  IUsersQueryService users)
  : ICommandHandler<SetUserStatusCommand, Result<UserListItemDto>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IRepository<Membership> _memberships = memberships;
  private readonly IUserAccountService _accounts = accounts;
  private readonly IUsersQueryService _users = users;

  public async ValueTask<Result<UserListItemDto>> Handle(SetUserStatusCommand command,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<UserListItemDto>.Unauthorized();

    var organizationId = OrganizationId.From(tenantId);

    // Suspender a si mesmo tira o proprio acesso na requisicao seguinte, e nao ha outra tela para
    // desfazer — quem fizesse isso sozinho numa organizacao a deixaria sem ninguem que pudesse
    // reativa-lo. O erro e' de conflito, nao de permissao: a permissao existe, a operacao e' que
    // nao faz sentido.
    if (!command.Active && _execution.UserId == command.UserId.Value)
    {
      return Result<UserListItemDto>.Conflict(
        "Nao e' possivel desativar o proprio acesso. Peca a outro gerenciador.");
    }

    var membership = await _memberships.FirstOrDefaultAsync(
      new MembershipByUserSpec(organizationId, command.UserId), cancellationToken);

    // Vinculo de outra organizacao cai aqui pela propria especificacao, e sai como 404 — nunca 403
    // (spec §16).
    if (membership is null) return Result<UserListItemDto>.NotFound();

    if (command.Active) membership.Activate();
    else membership.Deactivate();

    await _memberships.UpdateAsync(membership, cancellationToken);

    if (!command.Active)
    {
      // Sem isto, o cookie ja emitido continuaria valido ate expirar. `GET /auth/me` releria o
      // vinculo e recusaria, mas qualquer endpoint que confie so' na claim aceitaria a sessao
      // suspensa ate oito horas depois.
      await _accounts.InvalidateSessionsAsync(command.UserId, cancellationToken);
    }

    // Relê pela consulta da tela, e nao monta o DTO a partir do agregado: e' a mesma projecao que
    // `GET /users/{id}` devolve, entao a linha que o frontend recebe depois de ativar e' identica a
    // que ele receberia recarregando.
    var user = await _users.FindAsync(organizationId, command.UserId, cancellationToken);

    return user is null
      ? Result<UserListItemDto>.Error(new ErrorList([ErrorCodes.Unexpected],
          "Vinculo alterado nao pode ser lido de volta."))
      : user;
  }
}
