using LicitaEdital.Queries.Contracts.Companies;

namespace LicitaEdital.Api.CompanyProfile;

/// <summary>
/// `CompanyProfileApiItem` do contrato
/// (`private/cfe/company-profile/models/company-profile-api.model.ts`).
/// </summary>
public sealed record CompanyProfileResponse(
  string CompanyName,
  string Cnpj,
  string City,
  string State,
  string BusinessArea)
{
  public static CompanyProfileResponse From(CompanyProfileDto profile) => new(
    profile.CompanyName,
    profile.Cnpj,
    profile.City,
    profile.State,
    profile.BusinessArea);
}
