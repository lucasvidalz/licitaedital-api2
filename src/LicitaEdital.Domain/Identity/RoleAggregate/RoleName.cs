using Vogen;

namespace LicitaEdital.Domain.Identity.RoleAggregate;

[ValueObject<string>]
public readonly partial struct RoleName
{
  public const int MaxLength = 80;

  private static string NormalizeInput(string input) => input?.Trim() ?? string.Empty;

  private static Validation Validate(string value)
  {
    if (string.IsNullOrWhiteSpace(value)) return Validation.Invalid("Nome do papel e obrigatorio.");
    if (value.Length > MaxLength) return Validation.Invalid($"Nome do papel excede {MaxLength} caracteres.");
    return Validation.Ok;
  }
}
