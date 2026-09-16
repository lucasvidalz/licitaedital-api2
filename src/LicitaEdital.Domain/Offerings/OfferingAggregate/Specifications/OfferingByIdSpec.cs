using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Offerings.OfferingAggregate.Specifications;

/// <summary>
/// Uma oferta **dentro de uma organizacao**. A organizacao e' parametro obrigatorio, e nao filtro
/// opcional: e' esta consulta que `PUT /offerings/{id}` usa para achar o alvo, e uma versao "por id"
/// sem organizacao deixaria um cliente reescrever a oferta de outro conhecendo o id.
/// </summary>
public sealed class OfferingByIdSpec : SingleResultSpecification<Offering>
{
  public OfferingByIdSpec(OrganizationId organizationId, OfferingId offeringId)
  {
    Query.Where(offering => offering.OrganizationId == organizationId && offering.Id == offeringId);
  }
}
