namespace LicitaEdital.Queries.Contracts.Collections;

/// <summary>
/// Uma execucao do coletor, como o painel a le. Espelha `CollectionRunApiItem`
/// (`private/gfe/collections/models/collection-run-api.model.ts`).
///
/// <para>
/// <c>EndedAt</c> nao e' anulavel **aqui**, embora seja no agregado: a listagem so' devolve execucao
/// **concluida**. Ver <see cref="ICollectionRunsQueryService"/> para o motivo.
/// </para>
/// </summary>
public sealed record CollectionRunDto(
  Guid Id,
  DateTimeOffset StartedAt,
  DateTimeOffset EndedAt,
  int NewOpportunitiesCount,
  string Result,
  string? ErrorMessage);

/// <summary>
/// Envelope proprio de `GET /gfe/collections`: a pagina **mais** <see cref="LastSuccessfulRunAt"/>.
///
/// <para>
/// <b>Nao e' o `PagedList&lt;T&gt;` padrao</b>, e nao deveria ser: o "ultimo sucesso" nao pertence a
/// pagina — ele e' o mesmo esteja o operador na pagina 1 ou na 9, e enfiá-lo no envelope generico o
/// tornaria um campo opcional que toda outra listagem carregaria sem usar.
/// </para>
/// </summary>
public sealed record CollectionRunsDto(
  IReadOnlyList<CollectionRunDto> Items,
  int Page,
  int PageSize,
  int TotalItems,
  int TotalPages,
  DateTimeOffset? LastSuccessfulRunAt);
