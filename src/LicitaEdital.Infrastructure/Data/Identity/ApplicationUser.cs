using Microsoft.AspNetCore.Identity;

namespace LicitaEdital.Infrastructure.Data.Identity;

/// <summary>
/// Registro de identidade. Fica em Infrastructure, e nao no Core, porque e' infraestrutura de
/// autenticacao: hash de senha, carimbo de seguranca, tokens de recuperacao e contagem de bloqueio
/// sao mecanismo do ASP.NET Core Identity, nao regra de negocio deste produto (D-03).
///
/// O dominio conhece so o <c>UserId</c>. Area, papel e situacao moram em
/// <see cref="LicitaEdital.Core.Identity.MembershipAggregate.Membership"/>, porque dependem da
/// organizacao — o mesmo usuario pode ter vinculos diferentes.
///
/// <see cref="DisplayName"/> mora aqui, e nao no vinculo, porque e' o nome da pessoa e viaja em
/// `AuthUser.displayName` antes de qualquer organizacao estar escolhida.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
  public const int DisplayNameMaxLength = 120;

  /// <summary>Nome exibido. Corresponde a `displayName` de `AuthUser` e de `UserApiItem`.</summary>
  public string DisplayName { get; set; } = string.Empty;

  public DateTimeOffset CreatedAt { get; set; }
}
