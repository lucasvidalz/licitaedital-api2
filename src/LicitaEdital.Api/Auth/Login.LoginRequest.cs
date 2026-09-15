using FluentValidation;

namespace LicitaEdital.Api.Auth;

public class LoginRequest
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Valida so' a **forma**. Se a credencial confere e' decisao do caso de uso, e a resposta de
/// falha la' e' 401 generico — nunca "senha errada" ou "e-mail nao cadastrado".
/// </summary>
public class LoginValidator : Validator<LoginRequest>
{
  public LoginValidator()
  {
    RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(256);
    RuleFor(request => request.Password).NotEmpty().MaximumLength(256);
  }
}
