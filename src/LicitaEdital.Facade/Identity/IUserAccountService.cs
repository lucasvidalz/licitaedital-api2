using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Facade.Identity;

/// <summary>
/// O que a aplicacao precisa do provedor de identidade. Implementado em Infrastructure sobre o
/// ASP.NET Core Identity — hash de senha, politica de token e bloqueio sao mecanismo dele, nao
/// regra deste produto (D-03).
///
/// <para>
/// <b>Token carrega o usuario dentro.</b> O contrato do frontend manda so' `{ token, password }` em
/// `reset-password` e so' `{ token }` em `confirm-email` — sem id e sem e-mail. O Identity, por sua
/// vez, valida token **por usuario**. A conciliacao e' um token composto, opaco para quem o recebe:
/// a implementacao embute o id e o separa na volta. Quem chama nao sabe disso, e nao deveria.
/// </para>
/// </summary>
public interface IUserAccountService
{
  Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

  Task<UserAccount?> FindByIdAsync(UserId userId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Cria a conta. <c>Result.Invalid</c> quando a senha nao cumpre a politica ou o e-mail ja
  /// existe — os dois sao erro de entrada do usuario, nao falha do sistema.
  /// </summary>
  Task<Result<UserAccount>> CreateAsync(string email, string displayName, string password,
    CancellationToken cancellationToken = default);

  /// <summary>
  /// Confere a senha. Respeita o bloqueio por tentativa do Identity: chamadas erradas sucessivas
  /// travam a conta, o que este produto nao precisa reimplementar.
  /// </summary>
  Task<bool> CheckPasswordAsync(UserId userId, string password,
    CancellationToken cancellationToken = default);

  /// <summary>Token composto de redefinicao de senha, para entrar no link do e-mail.</summary>
  Task<string> GeneratePasswordResetTokenAsync(UserId userId,
    CancellationToken cancellationToken = default);

  /// <summary>
  /// Redefine a senha. Expiracao e uso unico sao do provedor — o `SecurityStamp` muda na troca e
  /// invalida o token usado, junto de qualquer sessao viva. E' o `DF-004` do `licitaledital-api`
  /// resolvido por mecanismo, nao por codigo nosso.
  /// </summary>
  Task<Result> ResetPasswordAsync(string token, string newPassword,
    CancellationToken cancellationToken = default);

  Task<string> GenerateEmailConfirmationTokenAsync(UserId userId,
    CancellationToken cancellationToken = default);

  Task<Result> ConfirmEmailAsync(string token, CancellationToken cancellationToken = default);

  /// <summary>Marca o acesso, para o `lastLoginAt` que `GET /users/{id}` expoe.</summary>
  Task RegisterSuccessfulLoginAsync(UserId userId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Derruba **toda** sessao aberta do usuario, em qualquer aba ou dispositivo.
  ///
  /// Troca o carimbo de seguranca do provedor, o que invalida cookie emitido antes e qualquer token
  /// de redefinicao pendente. E' o que faz suspender um vinculo ter efeito imediato, e nao so' na
  /// proxima carga da aplicacao.
  /// </summary>
  Task InvalidateSessionsAsync(UserId userId, CancellationToken cancellationToken = default);
}
