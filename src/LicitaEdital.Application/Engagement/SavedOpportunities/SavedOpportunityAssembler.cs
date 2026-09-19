using LicitaEdital.Facade.Catalog;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Engagement;

namespace LicitaEdital.Application.Engagement.SavedOpportunities;

/// <summary>
/// Junta as linhas de salvamento com as licitacoes da fachada. Um lugar so', porque listar e salvar
/// produzem a mesma forma — e divergir ali faria a linha recem-salva aparecer diferente da mesma
/// linha depois de recarregar.
/// </summary>
public static class SavedOpportunityAssembler
{
  public static async Task<IReadOnlyList<SavedOpportunityView>> AssembleAsync(
    ICatalogFacade catalog,
    OrganizationId organizationId,
    IReadOnlyList<SavedOpportunityRefDto> saved,
    CancellationToken cancellationToken)
  {
    if (saved.Count == 0) return [];

    // Uma chamada com todos os ids, nunca uma por item: N+1 atravessando fronteira de modulo e' o
    // pior lugar possivel para ele acontecer.
    var summaries = await catalog.GetSummariesAsync(organizationId,
      [.. saved.Select(row => OpportunityId.From(row.OpportunityId))], cancellationToken);

    var byId = summaries.ToDictionary(summary => summary.Id.Value);

    // A ordem e' a das linhas salvas (mais recente primeiro), nao a da fachada.
    //
    // **Licitacao ausente e' descartada em silencio**, e nao e' descuido: a fonte pode ter
    // despublicado o edital, e a fachada documenta que id inexistente simplesmente nao volta. Falhar
    // a listagem inteira por causa de uma linha orfa deixaria a tela vazia por um item.
    return
    [
      .. saved
        .Where(row => byId.ContainsKey(row.OpportunityId))
        .Select(row => new SavedOpportunityView(row.SavedAt, byId[row.OpportunityId]))
    ];
  }
}
