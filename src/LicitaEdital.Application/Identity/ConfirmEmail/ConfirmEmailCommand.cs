namespace LicitaEdital.Queries.Contracts.Identity.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Token) : ICommand<Result>;
