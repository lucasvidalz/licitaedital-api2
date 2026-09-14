using Vogen;

namespace LicitaEdital.Core.Offerings.OfferingAggregate;

[ValueObject<string>]
public readonly partial struct OfferingName
{
  public const int MaxLength = 200;

  private static string NormalizeInput(string input) => input?.Trim() ?? string.Empty;

  private static Validation Validate(string value)
  {
    if (string.IsNullOrWhiteSpace(value)) return Validation.Invalid("Nome da oferta e obrigatorio.");
    if (value.Length > MaxLength) return Validation.Invalid($"Nome da oferta excede {MaxLength} caracteres.");
    return Validation.Ok;
  }
}
