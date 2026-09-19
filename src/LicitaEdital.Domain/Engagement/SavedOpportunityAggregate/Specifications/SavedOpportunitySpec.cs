using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Engagement.SavedOpportunityAggregate.Specifications;

/// <summary>
/// O salvamento de uma licitacao por uma organizacao. Ha no maximo um — indice unico por
/// (organizacao, licitacao) —, e a organizacao e' parametro obrigatorio: sem ela, `DELETE` deixaria
/// um cliente dessalvar a licitacao de outro conhecendo o id.
/// </summary>
public sealed class SavedOpportunitySpec : SingleResultSpecification<SavedOpportunity>
{
  public SavedOpportunitySpec(OrganizationId organizationId, OpportunityId opportunityId)
  {
    Query.Where(saved => saved.OrganizationId == organizationId
                      && saved.OpportunityId == opportunityId);
  }
}
