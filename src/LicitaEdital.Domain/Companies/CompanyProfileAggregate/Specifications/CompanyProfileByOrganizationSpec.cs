using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Companies.CompanyProfileAggregate.Specifications;

/// <summary>
/// O perfil de uma organizacao. Ha no maximo um — `ux_company_profiles_organization` garante — e e'
/// o que faz `PUT /company-profile` ser upsert: existe, atualiza; nao existe, cria.
/// </summary>
public sealed class CompanyProfileByOrganizationSpec : SingleResultSpecification<CompanyProfile>
{
  public CompanyProfileByOrganizationSpec(OrganizationId organizationId)
  {
    Query.Where(profile => profile.OrganizationId == organizationId);
  }
}
