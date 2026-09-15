using Vogen;

namespace LicitaEdital.Domain.Catalog.CompatibilityAggregate;

/// <summary>
/// Nota de 0 a 100. O servidor devolve **o numero**; a faixa (`high`/`medium`/`low`/`poor`) e'
/// derivada no cliente (`opportunity.model.ts:1-7`). Nao persista nem devolva a faixa — duas fontes
/// para a mesma classificacao divergem na primeira mudanca de limiar.
/// </summary>
[ValueObject<int>]
public readonly partial struct CompatibilityScore
{
  public const int Min = 0;
  public const int Max = 100;

  private static Validation Validate(int value)
      => value is >= Min and <= Max ? Validation.Ok : Validation.Invalid($"Score deve estar entre {Min} e {Max}.");
}
