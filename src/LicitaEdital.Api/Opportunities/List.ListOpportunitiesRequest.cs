using FluentValidation;
using LicitaEdital.BuildingBlocks.Application.Paging;
using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.Domain.Catalog.OpportunityAggregate;

namespace LicitaEdital.Api.Opportunities;

/// <summary>
/// Os filtros do feed. <c>State</c> e <c>Modality</c> chegam **repetidos** na query string
/// (`?state=SP&amp;state=RJ`), como `build-opportunities-query.helper.ts` os monta — daí serem
/// coleções, e daí os nomes virem no singular.
/// </summary>
public class ListOpportunitiesRequest
{
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = PageRequest.DefaultPageSize;
  public string? Sort { get; set; }
  public string? Search { get; set; }
  public IReadOnlyList<string> State { get; set; } = [];
  public IReadOnlyList<string> Modality { get; set; } = [];
  public long? MinValueCents { get; set; }
  public long? MaxValueCents { get; set; }
}

/// <summary>
/// <b>`sort` invalido nao e' validado aqui de proposito.</b> <see cref="OpportunitySort.FromRequest"/>
/// cai no padrao (`score`) em vez de recusar: o feed e' a primeira tela do produto, e derruba-lo por
/// causa de um parametro de ordenacao seria desproporcional — a tela ainda tem o que mostrar, e o
/// cliente so' oferece as tres opcoes validas.
///
/// <para>
/// UF e faixa de valor **sao** validadas: UF desconhecida devolveria lista vazia, que se le como
/// "nao ha licitacao em SS" em vez de "SS nao existe"; e faixa invertida devolveria vazio para
/// sempre, sem que a tela soubesse por que.
/// </para>
/// </summary>
public class ListOpportunitiesValidator : Validator<ListOpportunitiesRequest>
{
  public ListOpportunitiesValidator()
  {
    RuleFor(request => request.Page).GreaterThanOrEqualTo(1);

    RuleFor(request => request.PageSize).InclusiveBetween(1, PageRequest.MaxPageSize);

    RuleFor(request => request.Search).MaximumLength(256);

    RuleFor(request => request.State)
      .Must(states => states.All(StateCode.IsValid))
      .WithMessage("Há UF inválida no filtro.");

    RuleFor(request => request.MinValueCents)
      .GreaterThanOrEqualTo(0).When(request => request.MinValueCents is not null);

    RuleFor(request => request.MaxValueCents)
      .GreaterThanOrEqualTo(0).When(request => request.MaxValueCents is not null);

    RuleFor(request => request)
      .Must(request => request.MinValueCents is null
                    || request.MaxValueCents is null
                    || request.MinValueCents <= request.MaxValueCents)
      .WithName(nameof(ListOpportunitiesRequest.MaxValueCents))
      .WithMessage("O valor máximo não pode ser menor que o mínimo.");
  }
}
