namespace LicitaEdital.Queries.Contracts.Companies;

/// <summary>
/// O perfil da empresa, como a tela o le. Espelha `CompanyProfileApiItem`
/// (`private/cfe/company-profile/models/company-profile-api.model.ts`) campo a campo.
///
/// <para>
/// **Nao carrega id.** O recurso e' singular por organizacao — `GET /company-profile` nao recebe id
/// — e devolver um id so' criaria a tentacao de aceita-lo de volta num `PUT`, que e' onde a troca de
/// tenant entraria (SEC-08).
/// </para>
/// </summary>
public sealed record CompanyProfileDto(
  string CompanyName,
  string Cnpj,
  string City,
  string State,
  string BusinessArea);
