namespace LicitaEdital.Core.Shared;

/// <summary>
/// Faixa de valor estimado, como chave simbolica. **Guarde a chave, nunca os centavos derivados**:
/// a conversao para centavos acontece no cliente
/// (`build-opportunities-query.helper.ts:11-17`), e persistir o intervalo congelaria a tabela de
/// faixas no dado historico.
/// </summary>
public sealed class ValueRange : SmartEnum<ValueRange, string>
{
  public static readonly ValueRange Unset = new(nameof(Unset), "", null, null);
  public static readonly ValueRange UpTo100K = new(nameof(UpTo100K), "up-to-100k", null, 10_000_000);
  public static readonly ValueRange From100KTo500K = new(nameof(From100KTo500K), "100k-500k", 10_000_000, 50_000_000);
  public static readonly ValueRange From500KTo1M = new(nameof(From500KTo1M), "500k-1m", 50_000_000, 100_000_000);
  public static readonly ValueRange Above1M = new(nameof(Above1M), "above-1m", 100_000_000, null);

  private ValueRange(string name, string value, long? minCents, long? maxCents) : base(name, value)
  {
    MinCents = minCents;
    MaxCents = maxCents;
  }

  /// <summary>Limite inferior em centavos, inclusivo. <c>null</c> quando nao ha piso.</summary>
  public long? MinCents { get; }

  /// <summary>Limite superior em centavos, exclusivo. <c>null</c> quando nao ha teto.</summary>
  public long? MaxCents { get; }
}
