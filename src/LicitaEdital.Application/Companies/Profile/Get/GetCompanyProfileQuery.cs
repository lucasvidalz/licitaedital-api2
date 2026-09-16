using LicitaEdital.Queries.Contracts.Companies;

namespace LicitaEdital.Application.Companies.Profile.Get;

/// <summary>
/// `GET /company-profile`. Sem parametro: o recurso e' singular por organizacao, e a organizacao vem
/// da sessao.
/// </summary>
public sealed record GetCompanyProfileQuery : IQuery<Result<CompanyProfileDto>>;
