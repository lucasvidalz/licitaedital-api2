using LicitaEdital.Core.Shared;

namespace LicitaEdital.UseCases.Identity;

/// <summary>
/// Monta o <see cref="AuthenticatedUserDto"/> de um usuario: acha o vinculo ativo, resolve o papel
/// e expande as permissoes.
///
/// E' consulta com junção entre tres agregados do mesmo modulo — nao cabe em
/// <c>IRepository&lt;T&gt;</c> sem tres idas ao banco. Implementado no projeto de Query.
/// </summary>
public interface IAuthenticatedUserReader
{
  /// <summary>
  /// <c>null</c> quando nao ha vinculo **ativo**: conta suspensa nao autentica, e uma sessao viva
  /// de quem foi suspenso para de resolver aqui.
  /// </summary>
  Task<AuthenticatedUserDto?> ReadAsync(UserId userId, CancellationToken cancellationToken = default);
}
