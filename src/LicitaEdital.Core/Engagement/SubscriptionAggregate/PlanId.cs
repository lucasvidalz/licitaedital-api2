namespace LicitaEdital.Core.Engagement.SubscriptionAggregate;

/// <summary>
/// Os 3 planos do catalogo (`plan-catalog.model.ts:16`). **Preco e recursos ficam no cliente** — o
/// catalogo e' estatico la e nao atravessa a API. O servidor so precisa saber em qual plano a
/// organizacao esta.
/// </summary>
public sealed class PlanId : SmartEnum<PlanId, string>
{
  public static readonly PlanId Inicial = new(nameof(Inicial), "inicial");
  public static readonly PlanId Profissional = new(nameof(Profissional), "profissional");
  public static readonly PlanId Consultor = new(nameof(Consultor), "consultor");

  private PlanId(string name, string value) : base(name, value) { }
}
