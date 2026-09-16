using FluentValidation;
using LicitaEdital.BuildingBlocks.Application.Paging;
using LicitaEdital.Domain.Identity.MembershipAggregate;

namespace LicitaEdital.Api.Users;

public class ListUsersRequest
{
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = PageRequest.DefaultPageSize;
  public string? Search { get; set; }
  public string? Status { get; set; }
}

/// <summary>
/// O teto de pagina e' **do servidor**. O cliente ja limita em 100
/// (`users-table.component.html`, opcoes 10/20/50), mas confiar nisso deixaria a API a um `curl` de
/// distancia de uma pagina com a base inteira.
///
/// <para>
/// <c>status</c> e' validado contra o catalogo fechado de <see cref="MembershipStatus"/>: valor
/// desconhecido responde 400, e nao uma lista vazia. Lista vazia para filtro invalido e' o erro que
/// faz alguem concluir que "nao ha usuarios inativos" quando digitou `inative`.
/// </para>
/// </summary>
public class ListUsersValidator : Validator<ListUsersRequest>
{
  public ListUsersValidator()
  {
    RuleFor(request => request.Page).GreaterThanOrEqualTo(1);

    RuleFor(request => request.PageSize)
      .InclusiveBetween(1, PageRequest.MaxPageSize);

    RuleFor(request => request.Search).MaximumLength(256);

    RuleFor(request => request.Status)
      .Must(status => status is null
                   || MembershipStatus.TryFromValue(status, out _))
      .WithMessage($"Situação inválida. Use '{MembershipStatus.Active.Value}' ou " +
                   $"'{MembershipStatus.Inactive.Value}'.");
  }
}
