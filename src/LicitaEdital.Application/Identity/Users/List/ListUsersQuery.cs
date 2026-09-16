namespace LicitaEdital.Application.Identity.Users.List;

/// <summary>
/// `GET /users`. **Nao carrega organizacao**: o tenant vem da sessao, dentro do handler. Aceita-lo
/// aqui abriria a porta para um cliente pedir a lista de outro (SEC-08).
/// </summary>
public sealed record ListUsersQuery(PageRequest Page, string? Search, string? Status)
  : IQuery<Result<PagedList<UserListItemDto>>>;
