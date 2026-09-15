using FluentValidation;

namespace LicitaEdital.Web.Auth;

public class RegisterRequest
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string DisplayName { get; set; } = string.Empty;
}

/// <summary>
/// Revalida no servidor o que o Angular ja valida como UX (SEC-30). O tamanho minimo de senha aqui
/// e' o piso; a politica completa e' do ASP.NET Identity, e o erro dele volta como
/// `ValidationProblem` com o codigo original — a tela decide o texto.
/// </summary>
public class RegisterValidator : Validator<RegisterRequest>
{
  public RegisterValidator()
  {
    RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(256);
    RuleFor(request => request.Password).NotEmpty().MinimumLength(8).MaximumLength(256);
    RuleFor(request => request.DisplayName).NotEmpty().MaximumLength(120);
  }
}
