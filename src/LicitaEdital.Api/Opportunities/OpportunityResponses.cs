using LicitaEdital.BuildingBlocks.Application.Paging;
using LicitaEdital.Queries.Contracts.Catalog;

namespace LicitaEdital.Api.Opportunities;

/// <summary>`OpportunityApiItem` do contrato (`opportunity-api.model.ts`).</summary>
public sealed record OpportunityResponse(
  Guid Id,
  string Title,
  string Object,
  string BuyerName,
  string State,
  string City,
  string? CityIbgeCode,
  ModalityResponse Modality,
  string Status,
  long? EstimatedValueCents,
  DateTimeOffset PublishedAt,
  DateTimeOffset? ProposalDeadline,
  string OfficialUrl,
  string Source,
  DateTimeOffset CollectedAt,
  CompatibilityResponse Compatibility)
{
  public static OpportunityResponse From(OpportunityListItemDto item) => new(
    item.Id, item.Title, item.Object, item.BuyerName, item.State, item.City, item.CityIbgeCode,
    new ModalityResponse(item.Modality.Code, item.Modality.Label), item.Status,
    item.EstimatedValueCents, item.PublishedAt, item.ProposalDeadline, item.OfficialUrl,
    item.Source, item.CollectedAt,
    new CompatibilityResponse(item.Compatibility.Score, item.Compatibility.OfferingId,
      item.Compatibility.MatchedTerms, item.Compatibility.PositiveReasons,
      item.Compatibility.AttentionPoints));
}

public sealed record ModalityResponse(string Code, string Label);

/// <summary>
/// `score` nulo e' `unrated` — estado real, nao erro. A **faixa** (`high`/`medium`/`low`/`poor`) e'
/// derivada no cliente e nao viaja daqui: duas fontes para a mesma classificacao divergem no
/// primeiro ajuste de limiar.
/// </summary>
public sealed record CompatibilityResponse(
  int? Score,
  Guid? OfferingId,
  IReadOnlyList<string> MatchedTerms,
  IReadOnlyList<string> PositiveReasons,
  IReadOnlyList<string> AttentionPoints);

/// <summary>`PagedResponse&lt;OpportunityApiItem&gt;` (`core/http/models/paged-response.model.ts`).</summary>
public sealed record PagedOpportunitiesResponse(
  IReadOnlyList<OpportunityResponse> Items,
  int Page,
  int PageSize,
  int TotalItems,
  int TotalPages)
{
  public static PagedOpportunitiesResponse From(PagedList<OpportunityListItemDto> page) => new(
    [.. page.Items.Select(OpportunityResponse.From)],
    page.Page, page.PageSize, page.TotalItems, page.TotalPages);
}

/// <summary>`OpportunityDetailApiItem` do contrato (`opportunity-detail-api.model.ts`).</summary>
public sealed record OpportunityDetailResponse(
  Guid Id,
  string Title,
  string Object,
  string BuyerName,
  string ContractNumber,
  string State,
  string City,
  string? CityIbgeCode,
  ModalityResponse Modality,
  long? EstimatedValueCents,
  DateTimeOffset PublishedAt,
  DateTimeOffset? ProposalDeadline,
  string OfficialUrl,
  string Source,
  DateTimeOffset CollectedAt,
  IReadOnlyList<OpportunityLineItemResponse> Items,
  IReadOnlyList<OpportunityDocumentResponse> Documents,
  OpportunityDetailCompatibilityResponse Compatibility)
{
  public static OpportunityDetailResponse From(OpportunityDetailDto detail) => new(
    detail.Id, detail.Title, detail.Object, detail.BuyerName, detail.ContractNumber, detail.State,
    detail.City, detail.CityIbgeCode,
    new ModalityResponse(detail.Modality.Code, detail.Modality.Label),
    detail.EstimatedValueCents, detail.PublishedAt, detail.ProposalDeadline, detail.OfficialUrl,
    detail.Source, detail.CollectedAt,
    [.. detail.Items.Select(item => new OpportunityLineItemResponse(item.Number, item.Description,
      item.Quantity, item.Unit, item.UnitValueCents, item.TotalValueCents, item.CatalogCode))],
    [.. detail.Documents.Select(document => new OpportunityDocumentResponse(document.Kind,
      document.Label, document.Url, document.PublishedAt))],
    new OpportunityDetailCompatibilityResponse(detail.Compatibility.Score,
      detail.Compatibility.OfferingId, detail.Compatibility.OfferingName,
      detail.Compatibility.MatchedTerms, detail.Compatibility.PositiveReasons,
      detail.Compatibility.AttentionPoints));
}

public sealed record OpportunityLineItemResponse(
  int Number,
  string Description,
  decimal Quantity,
  string Unit,
  long? UnitValueCents,
  long? TotalValueCents,
  string? CatalogCode);

public sealed record OpportunityDocumentResponse(
  string Kind,
  string Label,
  string Url,
  DateTimeOffset? PublishedAt);

public sealed record OpportunityDetailCompatibilityResponse(
  int? Score,
  Guid? OfferingId,
  string? OfferingName,
  IReadOnlyList<string> MatchedTerms,
  IReadOnlyList<string> PositiveReasons,
  IReadOnlyList<string> AttentionPoints);
