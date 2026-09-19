using LicitaEdital.Queries.Contracts.Collections;

namespace LicitaEdital.Application.Collections.Runs.List;

/// <summary>
/// `GET /gfe/collections`. **Sem tenant**: a coleta e' da plataforma, e quem protege o endpoint e' a
/// area `manager` mais a permissao `collections.read`.
/// </summary>
public sealed record ListCollectionRunsQuery(PageRequest Page) : IQuery<Result<CollectionRunsDto>>;
