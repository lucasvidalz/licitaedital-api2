namespace LicitaEdital.UseCases.Identity.ResetPassword;

public sealed record ResetPasswordCommand(string Token, string Password) : ICommand<Result>;
