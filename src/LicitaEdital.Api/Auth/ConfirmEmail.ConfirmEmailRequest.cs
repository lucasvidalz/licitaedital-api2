using FluentValidation;

namespace LicitaEdital.Api.Auth;

public class ConfirmEmailRequest
{
  public string Token { get; set; } = string.Empty;
}

public class ConfirmEmailValidator : Validator<ConfirmEmailRequest>
{
  public ConfirmEmailValidator()
  {
    RuleFor(request => request.Token).NotEmpty();
  }
}
