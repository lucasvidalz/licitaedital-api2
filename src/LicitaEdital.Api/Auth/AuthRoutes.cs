namespace LicitaEdital.Api.Auth;

/// <summary>
/// As 7 rotas de `/auth`, num lugar so. Os valores sao contrato — o `AuthApiService` do Angular as
/// chama literalmente.
/// </summary>
public static class AuthRoutes
{
  public const string Me = "/auth/me";
  public const string Login = "/auth/login";
  public const string Register = "/auth/register";
  public const string Logout = "/auth/logout";
  public const string ForgotPassword = "/auth/forgot-password";
  public const string ResetPassword = "/auth/reset-password";
  public const string ConfirmEmail = "/auth/confirm-email";
}
