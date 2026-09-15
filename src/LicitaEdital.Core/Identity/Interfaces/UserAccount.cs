using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Identity.Interfaces;

/// <summary>
/// Recorte do registro de identidade que o dominio precisa. **Nao e' entidade** — o registro
/// pertence ao ASP.NET Core Identity, em Infrastructure. Isto e' o que atravessa a fronteira.
///
/// Sem hash de senha, sem carimbo de seguranca, sem contador de bloqueio: a API devolve so' os
/// campos que a tela usa (SEC-64), e o que nao sai daqui nao vaza por descuido la' na frente.
/// </summary>
public sealed record UserAccount(UserId Id, string Email, string DisplayName);
