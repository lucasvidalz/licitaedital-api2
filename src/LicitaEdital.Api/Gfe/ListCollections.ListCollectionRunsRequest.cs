using FluentValidation;
using LicitaEdital.BuildingBlocks.Application.Paging;

namespace LicitaEdital.Api.Gfe;

public class ListCollectionRunsRequest
{
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = PageRequest.DefaultPageSize;
}

public class ListCollectionRunsValidator : Validator<ListCollectionRunsRequest>
{
  public ListCollectionRunsValidator()
  {
    RuleFor(request => request.Page).GreaterThanOrEqualTo(1);
    RuleFor(request => request.PageSize).InclusiveBetween(1, PageRequest.MaxPageSize);
  }
}
