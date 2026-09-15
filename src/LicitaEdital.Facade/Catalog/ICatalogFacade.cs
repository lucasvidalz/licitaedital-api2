using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Facade.Catalog;

/// <summary>
/// O que o modulo Catalog promete aos outros modulos. **E' a unica porta de entrada** — nenhum
/// modulo alcanca o schema `catalog`, nem por DbContext, nem por join (spec §4).
///
/// Quem usa hoje: Engagement, para montar `GET /saved-opportunities`, que devolve a oportunidade
/// inteira embutida em cada linha salva. Sem a fachada, Engagement precisaria de um join entre
/// `engagement.saved_opportunities` e `catalog.opportunities` — exatamente o acoplamento que a
/// separacao por modulo existe para impedir.
/// </summary>
public interface ICatalogFacade
{
  /// <summary>
  /// Resumo das licitacoes pedidas, com a compatibilidade calculada **para aquela organizacao**.
  ///
  /// Recebe a lista de ids de uma vez, e nao um id por chamada, porque o chamador tipico ja tem N
  /// referencias em maos — uma consulta por item seria N+1 atravessando a fronteira de modulo, que
  /// e' o pior lugar possivel para ele acontecer.
  ///
  /// Id que nao existe simplesmente nao aparece no resultado: cabe ao chamador decidir se a
  /// ausencia e' erro. Para oportunidade salva, nao e' — a licitacao pode ter sido despublicada.
  /// </summary>
  Task<IReadOnlyList<OpportunitySummary>> GetSummariesAsync(
    OrganizationId organizationId,
    IReadOnlyCollection<OpportunityId> opportunityIds,
    CancellationToken cancellationToken = default);
}
