namespace LicitaEdital.Application.Identity.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand<Result>;
