namespace LicitaEdital.Queries.Contracts.Identity;

/// <summary>
/// Um usuario na tela de gerenciamento. Espelha `UserApiItem`
/// (`private/gfe/users/models/user-api.model.ts`) campo a campo.
///
/// <para>
/// <b><c>Id</c> e' o id do usuario, nao o do vinculo.</b> E' o mesmo id de `GET /auth/me`, e e' o
/// que entra na rota `/gfe/users/{id}`. O vinculo e' detalhe de modelagem: a tela fala de pessoa,
/// e um id de vinculo mudaria se a pessoa fosse removida e readicionada a organizacao.
/// </para>
///
/// <para>
/// <c>LastLoginAt</c> e <c>CompanyName</c> sao anulaveis e **vao no corpo mesmo quando nulos**. O
/// contrato os declara opcionais (`lastLoginAt?`, `companyName?`), mas omitir a chave e mandar
/// `null` nao sao a mesma coisa para quem le: a tela distingue "nunca acessou" de "campo que a API
/// nao conhece".
/// </para>
/// </summary>
public sealed record UserListItemDto(
  Guid Id,
  string Email,
  string DisplayName,
  string Status,
  DateTimeOffset CreatedAt,
  DateTimeOffset? LastLoginAt,
  string? CompanyName);
