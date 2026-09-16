namespace LicitaEdital.Domain.Identity.MembershipAggregate;

/// <summary>
/// Area do usuario, que decide qual metade do produto ele enxerga: `client` roteia para /cfe,
/// `manager` para /gfe (`app.routes.ts:13,19`).
///
/// Quem define a area e' o backend, a partir da sessao — o login nao oferece escolha (AD-043 do
/// frontend), e os guards do Angular nao substituem a revalidacao no endpoint (SEC-07, SEC-29).
/// </summary>
public sealed class UserArea : SmartEnum<UserArea, string>
{
  /// <summary>
  /// Os mesmos valores, como constantes. Atributo de autorizacao precisa de constante em tempo de
  /// compilacao, e <c>UserArea.Manager.Value</c> nao e' uma — sem isto, o literal <c>"manager"</c>
  /// voltaria a ser digitado em cada endpoint, e um erro de digitacao la' nao da erro de
  /// compilacao: da uma policy que ninguem satisfaz.
  /// </summary>
  public static class Codes
  {
    public const string Client = "client";
    public const string Manager = "manager";
  }

  public static readonly UserArea Client = new(nameof(Client), Codes.Client);
  public static readonly UserArea Manager = new(nameof(Manager), Codes.Manager);

  private UserArea(string name, string value) : base(name, value) { }
}
