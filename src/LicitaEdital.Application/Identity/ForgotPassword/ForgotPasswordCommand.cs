namespace LicitaEdital.Queries.Contracts.Identity.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand<Result>;
