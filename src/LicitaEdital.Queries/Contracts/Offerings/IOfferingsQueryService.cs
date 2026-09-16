namespace LicitaEdital.Queries.Contracts.Offerings;

/// <summary>
/// Leitura das ofertas da organizacao.
///
/// Os dois metodos exigem a organizacao: oferta de outro tenant **nao existe** do ponto de vista de
/// quem pergunta, e <see cref="FindAsync"/> devolve <c>null</c> para o endpoint traduzir em 404 —
/// nunca 403 (spec §16).
/// </summary>
public interface IOfferingsQueryService
{
  /// <summary>Sem paginacao: o contrato devolve `OfferingApiItem[]` puro, nao um envelope.</summary>
  Task<IReadOnlyList<OfferingDto>> ListAsync(OrganizationId organizationId,
    CancellationToken cancellationToken = default);

  Task<OfferingDto?> FindAsync(OrganizationId organizationId, OfferingId offeringId,
    CancellationToken cancellationToken = default);
}
