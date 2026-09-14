namespace LicitaEdital.Core.Identity.MembershipAggregate;

/// <summary>
/// Area do usuario, que decide qual metade do produto ele enxerga: `client` roteia para /cfe,
/// `manager` para /gfe (`app.routes.ts:13,19`).
///
/// Quem define a area e' o backend, a partir da sessao — o login nao oferece escolha (AD-043 do
/// frontend), e os guards do Angular nao substituem a revalidacao no endpoint (SEC-07, SEC-29).
/// </summary>
public sealed class UserArea : SmartEnum<UserArea, string>
{
  public static readonly UserArea Client = new(nameof(Client), "client");
  public static readonly UserArea Manager = new(nameof(Manager), "manager");

  private UserArea(string name, string value) : base(name, value) { }
}
