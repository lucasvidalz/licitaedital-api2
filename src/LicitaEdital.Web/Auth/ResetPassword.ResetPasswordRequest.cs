using FluentValidation;

namespace LicitaEdital.Web.Auth;

public class ResetPasswordRequest
{
  public string Token { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}

public class ResetPasswordValidator : Validator<ResetPasswordRequest>
{
  public ResetPasswordValidator()
  {
    RuleFor(request => request.Token).NotEmpty();
    RuleFor(request => request.Password).NotEmpty().MinimumLength(8).MaximumLength(256);
  }
}
