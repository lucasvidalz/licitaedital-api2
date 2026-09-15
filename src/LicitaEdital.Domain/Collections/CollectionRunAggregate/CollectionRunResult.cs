namespace LicitaEdital.Domain.Collections.CollectionRunAggregate;

/// <summary>Espelha `result` de `CollectionRunApiItem` (`collection-run-api.model.ts:6`).</summary>
public sealed class CollectionRunResult : SmartEnum<CollectionRunResult, string>
{
  public static readonly CollectionRunResult Success = new(nameof(Success), "success");
  public static readonly CollectionRunResult Partial = new(nameof(Partial), "partial");
  public static readonly CollectionRunResult Failure = new(nameof(Failure), "failure");

  private CollectionRunResult(string name, string value) : base(name, value) { }
}
