using LicitaEdital.UseCases.Identity;

namespace LicitaEdital.Web.Auth;

/// <summary>
/// Espelho de `AuthUser` (`core/auth/models/auth-user.model.ts`). Os nomes sao contrato: o Angular
/// le campo a campo, e `area` decide para qual metade do produto ele roteia.
/// </summary>
public sealed record AuthUserResponse(
  string Id,
  string Email,
  string DisplayName,
  string Area,
  IReadOnlyList<string> Permissions)
{
  public static AuthUserResponse From(AuthenticatedUserDto user)
    => new(user.Id.ToString(), user.Email, user.DisplayName, user.Area, user.Permissions);
}
