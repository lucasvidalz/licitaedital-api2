namespace LicitaEdital.Core.Offerings.OfferingAggregate;

/// <summary>Espelha `supplyType` de `OfferingApiItem` (`offering-api.model.ts:8`).</summary>
public sealed class SupplyType : SmartEnum<SupplyType, string>
{
  public static readonly SupplyType Product = new(nameof(Product), "product");
  public static readonly SupplyType Service = new(nameof(Service), "service");
  public static readonly SupplyType Both = new(nameof(Both), "both");

  private SupplyType(string name, string value) : base(name, value) { }
}
