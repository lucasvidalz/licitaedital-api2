using LicitaEdital.Queries.Contracts.Offerings;

namespace LicitaEdital.Application.Offerings.List;

/// <summary>`GET /offerings`. Tudo da organizacao da sessao, sem paginacao e sem filtro.</summary>
public sealed record ListOfferingsQuery : IQuery<Result<IReadOnlyList<OfferingDto>>>;
