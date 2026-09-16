using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Offerings.OfferingAggregate.Specifications;

/// <summary>
/// A oferta de um nome dentro da organizacao. Serve para detectar a colisao de
/// `ux_offerings_organization_name` antes do banco recusar — violacao de indice vira excecao e 500,
/// e o formulario precisa de um erro no campo.
/// </summary>
public sealed class OfferingByNameSpec : SingleResultSpecification<Offering>
{
  public OfferingByNameSpec(OrganizationId organizationId, OfferingName name)
  {
    Query.Where(offering => offering.OrganizationId == organizationId && offering.Name == name);
  }
}
