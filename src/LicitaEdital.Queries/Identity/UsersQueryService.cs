using LicitaEdital.Queries.Contracts.Identity;

namespace LicitaEdital.Queries.Identity;

/// <summary>
/// A consulta da tela de usuarios, sobre a projecao <see cref="UserDirectoryRow"/>.
/// </summary>
public class UsersQueryService(IdentityReadContext context) : IUsersQueryService
{
  private readonly IdentityReadContext _context = context;

  public async Task<PagedList<UserListItemDto>> ListAsync(ListUsersFilter filter, PageRequest page,
    CancellationToken cancellationToken = default)
  {
    var rows = Rows(filter.OrganizationId);

    rows = rows.WhereIf(!string.IsNullOrWhiteSpace(filter.Search),
      // ILIKE nos dois campos que a tabela mostra. `ToLower().Contains()` produziria
      // `LOWER(coluna) LIKE ...`, que cega o indice comum da coluna — e o caminho daqui, com
      // volume, e' um indice GIN de trigrama, que o `LOWER` tambem nao usaria.
      row => EF.Functions.ILike(row.DisplayName, $"%{filter.Search}%")
          || EF.Functions.ILike(row.Email, $"%{filter.Search}%"));

    rows = rows.WhereIf(!string.IsNullOrWhiteSpace(filter.Status),
      row => row.Status == filter.Status);

    // Mais recente primeiro, com o id como desempate. Sem o desempate, duas contas criadas no mesmo
    // instante podem trocar de lugar entre uma pagina e outra, e a linha que muda de posicao some
    // ou aparece duas vezes para quem esta paginando.
    var ordered = rows
      .OrderByDescending(row => row.CreatedAt)
      .ThenByDescending(row => row.UserId);

    var pageResult = await ordered.ToPagedListAsync(page, cancellationToken);

    return pageResult.Map(ToDto);
  }

  public async Task<UserListItemDto?> FindAsync(OrganizationId organizationId, UserId userId,
    CancellationToken cancellationToken = default)
  {
    var row = await Rows(organizationId)
      .FirstOrDefaultAsync(candidate => candidate.UserId == userId.Value, cancellationToken);

    return row is null ? null : ToDto(row);
  }

  /// <summary>
  /// A consulta base, com o filtro de organizacao **dentro** — e nao acrescentado por quem chama.
  ///
  /// <para>
  /// E' o ponto unico onde o isolamento entre clientes e' aplicado nesta tela, e o motivo de os dois
  /// metodos publicos exigirem a organizacao na assinatura: nao existe caminho para consultar sem
  /// ela. A spec §16 trata filtro global como camada adicional, nunca como a principal — um
  /// `IgnoreQueryFilters` escrito sem pensar nao pode ser o que separa uma empresa da outra.
  /// </para>
  /// </summary>
  private IQueryable<UserDirectoryRow> Rows(OrganizationId organizationId)
    => _context.UserDirectory.Where(row => row.OrganizationId == organizationId.Value);

  private static UserListItemDto ToDto(UserDirectoryRow row) => new(
    row.UserId,
    row.Email,
    row.DisplayName,
    row.Status,
    row.CreatedAt,
    row.LastLoginAt,
    row.CompanyName);
}
