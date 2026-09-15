using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Catalog.CompatibilityAggregate.Specifications;

/// <summary>Compatibilidades calculadas para uma organizacao. Usada ao invalidar por troca de oferta.</summary>
public sealed class CompatibilityByOrganizationSpec : Specification<OpportunityCompatibility>
{
  public CompatibilityByOrganizationSpec(OrganizationId organizationId)
  {
    Query.Where(compatibility => compatibility.OrganizationId == organizationId);
  }
}

/// <summary>Compatibilidades de uma licitacao, de **todas** as organizacoes.</summary>
public sealed class CompatibilityByOpportunitySpec : Specification<OpportunityCompatibility>
{
  public CompatibilityByOpportunitySpec(OpportunityId opportunityId)
  {
    Query.Where(compatibility => compatibility.OpportunityId == opportunityId);
  }
}
