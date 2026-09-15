using System.Buffers.Text;
using System.Text;
using LicitaEdital.Core.Identity.Interfaces;
using LicitaEdital.Core.Shared;
using Microsoft.AspNetCore.Identity;

namespace LicitaEdital.Infrastructure.Data.Identity;

/// <summary>
/// <see cref="IUserAccountService"/> sobre o ASP.NET Core Identity.
/// </summary>
public class UserAccountService(UserManager<ApplicationUser> userManager, TimeProvider clock)
  : IUserAccountService
{
  /// <summary>
  /// Separador do token composto. <c>|</c> nao aparece em Guid nem na saida base64url do Identity,
  /// entao dividir pela **primeira** ocorrencia e' sempre correto.
  /// </summary>
  private const char TokenSeparator = '|';

  private readonly UserManager<ApplicationUser> _userManager = userManager;
  private readonly TimeProvider _clock = clock;

  public async Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByEmailAsync(email);
    return user is null ? null : ToAccount(user);
  }

  public async Task<UserAccount?> FindByIdAsync(UserId userId, CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByIdAsync(userId.Value.ToString());
    return user is null ? null : ToAccount(user);
  }

  public async Task<Result<UserAccount>> CreateAsync(string email, string displayName,
    string password, CancellationToken cancellationToken = default)
  {
    var user = new ApplicationUser
    {
      Id = Guid.CreateVersion7(),
      UserName = email,
      Email = email,
      DisplayName = displayName,
      CreatedAt = _clock.GetUtcNow()
    };

    var result = await _userManager.CreateAsync(user, password);

    return result.Succeeded
      ? ToAccount(user)
      : Result<UserAccount>.Invalid(ToValidationErrors(result));
  }

  public async Task<bool> CheckPasswordAsync(UserId userId, string password,
    CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByIdAsync(userId.Value.ToString());
    if (user is null) return false;

    // Conta bloqueada nao autentica **mesmo com a senha certa** — e' o bloqueio por tentativa do
    // Identity fazendo o trabalho que este produto nao precisa reimplementar.
    if (await _userManager.IsLockedOutAsync(user)) return false;

    var valid = await _userManager.CheckPasswordAsync(user, password);

    if (valid)
    {
      await _userManager.ResetAccessFailedCountAsync(user);
    }
    else
    {
      await _userManager.AccessFailedAsync(user);
    }

    return valid;
  }

  public async Task<string> GeneratePasswordResetTokenAsync(UserId userId,
    CancellationToken cancellationToken = default)
  {
    var user = await RequireUserAsync(userId);
    return Compose(userId, await _userManager.GeneratePasswordResetTokenAsync(user));
  }

  public async Task<Result> ResetPasswordAsync(string token, string newPassword,
    CancellationToken cancellationToken = default)
  {
    if (!TryDecompose(token, out var userId, out var identityToken)) return Result.Invalid(InvalidToken());

    var user = await _userManager.FindByIdAsync(userId.ToString());
    // Token de usuario inexistente e token expirado devolvem a **mesma** coisa: distinguir os dois
    // transformaria a tela de redefinicao num verificador de contas.
    if (user is null) return Result.Invalid(InvalidToken());

    var result = await _userManager.ResetPasswordAsync(user, identityToken, newPassword);

    return result.Succeeded ? Result.Success() : Result.Invalid(ToValidationErrors(result));
  }

  public async Task<string> GenerateEmailConfirmationTokenAsync(UserId userId,
    CancellationToken cancellationToken = default)
  {
    var user = await RequireUserAsync(userId);
    return Compose(userId, await _userManager.GenerateEmailConfirmationTokenAsync(user));
  }

  public async Task<Result> ConfirmEmailAsync(string token, CancellationToken cancellationToken = default)
  {
    if (!TryDecompose(token, out var userId, out var identityToken)) return Result.Invalid(InvalidToken());

    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null) return Result.Invalid(InvalidToken());

    var result = await _userManager.ConfirmEmailAsync(user, identityToken);

    return result.Succeeded ? Result.Success() : Result.Invalid(InvalidToken());
  }

  public async Task RegisterSuccessfulLoginAsync(UserId userId, CancellationToken cancellationToken = default)
  {
    var user = await _userManager.FindByIdAsync(userId.Value.ToString());
    if (user is null) return;

    await _userManager.ResetAccessFailedCountAsync(user);
  }

  // ------------------------------------------------------------------ token composto

  /// <summary>
  /// <c>base64url(userId|tokenDoIdentity)</c>. O contrato do frontend manda so' o token, e o
  /// Identity valida token por usuario — o id viaja junto, escondido de quem le o link.
  ///
  /// Nao e' segredo nem assinatura: quem alterar o id so' consegue um token que nao valida, porque
  /// a verificacao continua sendo a do Identity.
  /// </summary>
  private static string Compose(UserId userId, string identityToken)
    => Base64Url.EncodeToString(
      Encoding.UTF8.GetBytes($"{userId.Value}{TokenSeparator}{identityToken}"));

  private static bool TryDecompose(string token, out Guid userId, out string identityToken)
  {
    userId = Guid.Empty;
    identityToken = string.Empty;

    if (string.IsNullOrWhiteSpace(token)) return false;

    string decoded;
    try
    {
      decoded = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(token));
    }
    catch (FormatException)
    {
      return false;
    }

    var separator = decoded.IndexOf(TokenSeparator);
    if (separator <= 0 || separator == decoded.Length - 1) return false;

    if (!Guid.TryParse(decoded[..separator], out userId)) return false;

    identityToken = decoded[(separator + 1)..];
    return true;
  }

  // ------------------------------------------------------------------ apoio

  private async Task<ApplicationUser> RequireUserAsync(UserId userId)
    => await _userManager.FindByIdAsync(userId.Value.ToString())
       ?? throw new InvalidOperationException($"Usuario {userId.Value} nao encontrado.");

  private static UserAccount ToAccount(ApplicationUser user)
    => new(UserId.From(user.Id), user.Email ?? string.Empty, user.DisplayName);

  private static List<ValidationError> InvalidToken() =>
    [new() { Identifier = "token", ErrorMessage = "Token inválido ou expirado.", ErrorCode = "token.invalid" }];

  /// <summary>
  /// Traduz o erro do Identity para <c>ValidationError</c>. O <c>Code</c> vai junto para o cliente
  /// decidir a mensagem — `PasswordTooShort` e `DuplicateEmail` pedem textos diferentes na tela.
  /// </summary>
  private static List<ValidationError> ToValidationErrors(IdentityResult result)
    => [.. result.Errors.Select(error => new ValidationError
    {
      Identifier = error.Code.Contains("Password", StringComparison.Ordinal) ? "password" : "email",
      ErrorMessage = error.Description,
      ErrorCode = error.Code
    })];
}
