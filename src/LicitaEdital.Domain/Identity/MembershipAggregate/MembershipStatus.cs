namespace LicitaEdital.Domain.Identity.MembershipAggregate;

/// <summary>
/// Espelha `UserAccountStatus` do contrato (`user.model.ts:1`), consumido por
/// `GET /users?status=` e pelos endpoints de ativar/desativar.
/// </summary>
public sealed class MembershipStatus : SmartEnum<MembershipStatus, string>
{
  public static readonly MembershipStatus Active = new(nameof(Active), "active");
  public static readonly MembershipStatus Inactive = new(nameof(Inactive), "inactive");

  private MembershipStatus(string name, string value) : base(name, value) { }
}
