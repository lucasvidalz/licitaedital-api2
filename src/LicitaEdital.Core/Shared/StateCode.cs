using Vogen;

namespace LicitaEdital.Core.Shared;

/// <summary>
/// Unidade federativa, sempre em duas letras maiusculas. O frontend filtra oportunidade por este
/// valor (`?state=SP&amp;state=RJ`) e o guarda em preferencia de alerta e cobertura de coleta.
/// </summary>
[ValueObject<string>]
public readonly partial struct StateCode
{
  public const int Length = 2;

  private static string NormalizeInput(string input) => input?.Trim().ToUpperInvariant() ?? string.Empty;

  private static Validation Validate(string value)
  {
    if (value.Length != Length) return Validation.Invalid("UF deve ter exatamente 2 letras.");
    if (!value.All(char.IsAsciiLetterUpper)) return Validation.Invalid("UF aceita apenas letras.");
    return Validation.Ok;
  }
}
