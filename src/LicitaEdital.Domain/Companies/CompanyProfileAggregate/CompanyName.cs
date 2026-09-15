using Vogen;

namespace LicitaEdital.Domain.Companies.CompanyProfileAggregate;

[ValueObject<string>]
public readonly partial struct CompanyName
{
  public const int MaxLength = 200;

  private static string NormalizeInput(string input) => input?.Trim() ?? string.Empty;

  private static Validation Validate(string value)
  {
    if (string.IsNullOrWhiteSpace(value)) return Validation.Invalid("Razao social e obrigatoria.");
    if (value.Length > MaxLength) return Validation.Invalid($"Razao social excede {MaxLength} caracteres.");
    return Validation.Ok;
  }
}
