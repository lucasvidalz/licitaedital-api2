namespace LicitaEdital.Api.Users;

/// <summary>
/// As 4 rotas de `/users`. Os valores sao contrato — o `UsersApiService` do Angular as monta a
/// partir da constante `PATH = 'users'` (`private/gfe/users/data-access/users-api.service.ts`).
/// </summary>
public static class UsersRoutes
{
  public const string List = "/users";
  public const string Get = "/users/{id}";
  public const string Activate = "/users/{id}/activate";
  public const string Deactivate = "/users/{id}/deactivate";
}
