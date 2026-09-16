namespace LicitaEdital.Application.Identity.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Token) : ICommand<Result>;
