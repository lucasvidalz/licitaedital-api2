using Vogen;

namespace LicitaEdital.Domain.Identity.OrganizationAggregate;

[ValueObject<string>]
public readonly partial struct OrganizationName
{
  public const int MaxLength = 200;

  private static string NormalizeInput(string input) => input?.Trim() ?? string.Empty;

  private static Validation Validate(string value)
  {
    if (string.IsNullOrWhiteSpace(value)) return Validation.Invalid("Nome da organizacao e obrigatorio.");
    if (value.Length > MaxLength) return Validation.Invalid($"Nome da organizacao excede {MaxLength} caracteres.");
    return Validation.Ok;
  }
}
