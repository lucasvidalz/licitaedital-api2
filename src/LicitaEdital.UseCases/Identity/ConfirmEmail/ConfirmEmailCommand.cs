namespace LicitaEdital.UseCases.Identity.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Token) : ICommand<Result>;
