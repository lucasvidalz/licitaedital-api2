using Vogen;

namespace LicitaEdital.Core.Companies.CompanyProfileAggregate;

/// <summary>
/// CNPJ armazenado sem mascara, 14 digitos, com digito verificador conferido. A tela envia
/// formatado; normalizar na fronteira do value object evita que o mesmo CNPJ entre duas vezes com
/// pontuacao diferente e escape da unicidade.
/// </summary>
[ValueObject<string>]
public readonly partial struct Cnpj
{
  public const int Length = 14;

  private static string NormalizeInput(string input)
      => new string((input ?? string.Empty).Where(char.IsAsciiDigit).ToArray());

  private static Validation Validate(string value)
  {
    if (value.Length != Length) return Validation.Invalid("CNPJ deve ter 14 digitos.");
    if (value.Distinct().Count() == 1) return Validation.Invalid("CNPJ invalido.");
    if (!HasValidCheckDigits(value)) return Validation.Invalid("CNPJ invalido.");
    return Validation.Ok;
  }

  private static bool HasValidCheckDigits(string digits)
  {
    int[] firstWeights = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    int[] secondWeights = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    var first = CheckDigit(digits, firstWeights);
    if (digits[12] - '0' != first) return false;

    var second = CheckDigit(digits, secondWeights);
    return digits[13] - '0' == second;
  }

  private static int CheckDigit(string digits, int[] weights)
  {
    var sum = 0;
    for (var i = 0; i < weights.Length; i++)
    {
      sum += (digits[i] - '0') * weights[i];
    }
    var remainder = sum % 11;
    return remainder < 2 ? 0 : 11 - remainder;
  }
}
