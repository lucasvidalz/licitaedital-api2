namespace LicitaEdital.Queries.Contracts.Identity;

/// <summary>
/// Leitura da tela de gerenciamento de usuarios (`GET /users`, `GET /users/{id}`).
///
/// Os dois metodos exigem a organizacao: recurso de outro tenant **nao existe** do ponto de vista
/// de quem pergunta, e e' por isso que <see cref="FindAsync"/> devolve <c>null</c> em vez de lancar
/// — o endpoint traduz isso para 404, nunca 403 (spec §16).
/// </summary>
public interface IUsersQueryService
{
  Task<PagedList<UserListItemDto>> ListAsync(ListUsersFilter filter, PageRequest page,
    CancellationToken cancellationToken = default);

  Task<UserListItemDto?> FindAsync(OrganizationId organizationId, UserId userId,
    CancellationToken cancellationToken = default);
}
