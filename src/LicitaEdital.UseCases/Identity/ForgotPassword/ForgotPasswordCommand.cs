namespace LicitaEdital.UseCases.Identity.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand<Result>;
