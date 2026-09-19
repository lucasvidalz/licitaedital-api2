using LicitaEdital.Domain.Collections.CollectionRunAggregate;
using LicitaEdital.Queries.Contracts.Collections;

namespace LicitaEdital.Queries.Collections;

public class CollectionRunsQueryService(CollectionsReadContext context) : ICollectionRunsQueryService
{
  private readonly CollectionsReadContext _context = context;

  public async Task<CollectionRunsDto> ListAsync(PageRequest page,
    CancellationToken cancellationToken = default)
  {
    var finished = _context.CollectionRuns.Where(run => run.EndedAt != null);

    var pageResult = await finished
      .OrderByDescending(run => run.StartedAt)
      .ThenByDescending(run => run.Id)
      .ToPagedListAsync(page, cancellationToken);

    // `lastSuccessfulRunAt` e' **derivado**, nao um contador a parte: o maior `EndedAt` entre as
    // execucoes bem-sucedidas. Guardar num campo proprio criaria duas verdades para a mesma coisa, e
    // elas divergiriam na primeira falha parcial.
    //
    // Consulta separada de proposito: ele nao pertence a pagina — e' o mesmo esteja o operador na
    // pagina 1 ou na 9, e calcular por pagina daria respostas diferentes para a mesma pergunta.
    var lastSuccess = await _context.CollectionRuns
      .Where(run => run.Result == CollectionRunResult.Success && run.EndedAt != null)
      .MaxAsync(run => (DateTimeOffset?)run.EndedAt, cancellationToken);

    return new CollectionRunsDto(
      [.. pageResult.Items.Select(ToDto)],
      pageResult.Page,
      pageResult.PageSize,
      pageResult.TotalItems,
      pageResult.TotalPages,
      lastSuccess);
  }

  private static CollectionRunDto ToDto(CollectionRun run) => new(
    run.Id.Value,
    run.StartedAt,
    // O filtro garante que nao e' nulo; o `!` documenta que a garantia esta na consulta, nao no tipo.
    run.EndedAt!.Value,
    run.NewOpportunitiesCount,
    run.Result.Value,
    run.ErrorMessage);
}

public class CoverageSettingsQueryService(CollectionsReadContext context)
  : ICoverageSettingsQueryService
{
  private readonly CollectionsReadContext _context = context;

  public async Task<CoverageSettingsDto> GetAsync(CancellationToken cancellationToken = default)
  {
    var settings = await _context.CoverageSettings.FirstOrDefaultAsync(cancellationToken);

    // Sem linha gravada, o padrao vem do **agregado**, nao daqui: o valor default num lugar so' e' o
    // que impede a tela mostrar uma coisa antes de salvar e outra depois.
    return ToDto(settings ?? Domain.Collections.CoverageSettingsAggregate.CoverageSettings.CreateDefault());
  }

  internal static CoverageSettingsDto ToDto(
    Domain.Collections.CoverageSettingsAggregate.CoverageSettings settings)
    => new([.. settings.AttendedStates.Select(state => state.Value)], settings.ValueRange.Value);
}
