using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Application.Identity.Users.SetStatus;

/// <summary>
/// `POST /users/{id}/activate` e `POST /users/{id}/deactivate`.
///
/// <para>
/// **Um comando para os dois endpoints**, e nao um por rota: a unica diferenca entre eles e' a
/// situacao alvo e a consequencia de derrubar as sessoes. Duplicar o handler duplicaria tambem a
/// checagem de tenant e a de auto-suspensao — e as duas so' precisam falhar uma vez para o dano
/// estar feito.
/// </para>
/// </summary>
public sealed record SetUserStatusCommand(UserId UserId, bool Active)
  : ICommand<Result<UserListItemDto>>;
