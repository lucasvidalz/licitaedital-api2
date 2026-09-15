using FluentValidation;

namespace LicitaEdital.Web.Auth;

public class ForgotPasswordRequest
{
  public string Email { get; set; } = string.Empty;
}

public class ForgotPasswordValidator : Validator<ForgotPasswordRequest>
{
  public ForgotPasswordValidator()
  {
    RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(256);
  }
}
