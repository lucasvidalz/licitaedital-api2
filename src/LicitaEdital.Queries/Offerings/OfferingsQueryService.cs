using LicitaEdital.Domain.Offerings.OfferingAggregate;
using LicitaEdital.Queries.Contracts.Offerings;

namespace LicitaEdital.Queries.Offerings;

public class OfferingsQueryService(OfferingsReadContext context) : IOfferingsQueryService
{
  private readonly OfferingsReadContext _context = context;

  public async Task<IReadOnlyList<OfferingDto>> ListAsync(OrganizationId organizationId,
    CancellationToken cancellationToken = default)
  {
    // Sem paginacao de proposito: o contrato devolve `OfferingApiItem[]` puro. Uma empresa tem
    // dezenas de ofertas, nao milhares — e a listagem e' a entrada do formulario, nao um feed.
    var offerings = await Scoped(organizationId)
      .OrderByDescending(offering => offering.CreatedAt)
      .ThenByDescending(offering => offering.Id)
      .ToListAsync(cancellationToken);

    return [.. offerings.Select(ToDto)];
  }

  public async Task<OfferingDto?> FindAsync(OrganizationId organizationId, OfferingId offeringId,
    CancellationToken cancellationToken = default)
  {
    var offering = await Scoped(organizationId)
      .FirstOrDefaultAsync(candidate => candidate.Id == offeringId, cancellationToken);

    return offering is null ? null : ToDto(offering);
  }

  /// <summary>
  /// A consulta base, com o filtro de organizacao dentro. Ponto unico do isolamento entre clientes
  /// nesta tela — a spec §16 trata filtro global como camada adicional, nunca como a principal.
  /// </summary>
  private IQueryable<Offering> Scoped(OrganizationId organizationId)
    => _context.Offerings.Where(offering => offering.OrganizationId == organizationId);

  private static OfferingDto ToDto(Offering offering) => new(
    offering.Id.Value,
    offering.Name.Value,
    offering.Description,
    [.. offering.PositiveKeywords],
    [.. offering.NegativeKeywords],
    [.. offering.Synonyms],
    offering.SupplyType.Value,
    [.. offering.CatalogCodes],
    offering.MinValueCents,
    offering.MaxValueCents,
    [.. offering.ServedRegions.Select(region => region.Value)],
    offering.CreatedAt);
}
