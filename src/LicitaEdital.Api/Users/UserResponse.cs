using LicitaEdital.BuildingBlocks.Application.Paging;
using LicitaEdital.Queries.Contracts.Identity;

namespace LicitaEdital.Api.Users;

/// <summary>
/// `UserApiItem` do contrato (`private/gfe/users/models/user-api.model.ts`).
///
/// Existe separado do <see cref="UserListItemDto"/> pelo mesmo motivo de `AuthUserResponse`: o DTO e'
/// da aplicacao e pode crescer; esta e' a forma que sai na rede, e cada campo novo aqui e' uma
/// decisao, nao um efeito colateral.
/// </summary>
public sealed record UserResponse(
  Guid Id,
  string Email,
  string DisplayName,
  string Status,
  DateTimeOffset CreatedAt,
  DateTimeOffset? LastLoginAt,
  string? CompanyName)
{
  public static UserResponse From(UserListItemDto user) => new(
    user.Id,
    user.Email,
    user.DisplayName,
    user.Status,
    user.CreatedAt,
    user.LastLoginAt,
    user.CompanyName);
}

/// <summary>
/// `PagedResponse&lt;UserApiItem&gt;` (`core/http/models/paged-response.model.ts`). Os nomes dos
/// campos sao contrato: renomear qualquer um quebra a paginacao da tela sem erro de compilacao de
/// nenhum dos dois lados.
/// </summary>
public sealed record PagedUsersResponse(
  IReadOnlyList<UserResponse> Items,
  int Page,
  int PageSize,
  int TotalItems,
  int TotalPages)
{
  public static PagedUsersResponse From(PagedList<UserListItemDto> page) => new(
    [.. page.Items.Select(UserResponse.From)],
    page.Page,
    page.PageSize,
    page.TotalItems,
    page.TotalPages);
}
