namespace LicitaEdital.UseCases.Identity;

/// <summary>
/// Usuario da sessao, do ponto de vista da aplicacao.
///
/// <para>
/// E' **maior que o contrato**: <see cref="TenantId"/> nao existe em `AuthUser`
/// (`core/auth/models/auth-user.model.ts`) e nao deve existir — a organizacao e' resolvida pelo
/// servidor a cada requisicao, pela claim da sessao, e mandar para o cliente so' criaria a tentacao
/// de aceita-la de volta num payload (SEC-08). Ela vive aqui porque o endpoint precisa dela para
/// montar a sessao, e morre na traducao para `AuthUserResponse`.
/// </para>
///
/// <c>Area</c> e <c>Permissions</c> saem do vinculo, nunca do cliente. O login nao oferece escolha
/// de area (AD-043 do frontend).
/// </summary>
public sealed record AuthenticatedUserDto(
  Guid Id,
  string Email,
  string DisplayName,
  Guid TenantId,
  string Area,
  IReadOnlyList<string> Permissions);
