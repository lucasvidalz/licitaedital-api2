using LicitaEdital.Core.Shared;

namespace LicitaEdital.UseCases.Catalog.Opportunities.List;

/// <summary>
/// A listagem do feed **fura o repositorio**, e este e' o caso que justifica a excecao: ela filtra
/// por cinco criterios, ordena por tres chaves diferentes, pagina, e junta a projecao de
/// compatibilidade — tudo numa consulta so. Montar isso por `IRepository&lt;Opportunity&gt;` traria
/// o agregado inteiro, com itens e documentos que a tela nao usa, para descartar em memoria.
///
/// A interface fica aqui, em UseCases; a implementacao vive no projeto <c>LicitaEdital.Query</c>,
/// sobre um contexto de leitura sem rastreamento. O handler nao sabe qual das duas coisas o atende.
/// </summary>
public interface IListOpportunitiesQueryService
{
  Task<PagedResult<OpportunityListItemDto>> ListAsync(
    OrganizationId organizationId,
    ListOpportunitiesFilter filter,
    OpportunitySort sort,
    PageRequest page,
    CancellationToken cancellationToken = default);
}
