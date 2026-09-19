using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Catalog.OpportunityAggregate.Specifications;

/// <summary>
/// Licitacoes com os itens carregados. E' o insumo do motor de compatibilidade: descricao e codigo
/// de catalogo dos itens entram no calculo, e sem o `Include` cada linha dispararia uma consulta
/// propria no meio do laco.
///
/// <para>
/// <b>O `Include` e' pelo nome do campo, nao pela propriedade.</b> <c>Opportunity.Items</c> e' uma
/// projecao de leitura (<c>_items.AsReadOnly()</c>) e esta marcada <c>Ignore</c> no mapeamento; a
/// navegacao de verdade e' o campo <c>_items</c>. Um <c>Include(o =&gt; o.Items)</c> compila e falha
/// em tempo de execucao com "is invalid inside an Include operation".
/// </para>
/// </summary>
public sealed class OpportunityWithItemsSpec : Specification<Opportunity>
{
  public OpportunityWithItemsSpec()
  {
    Query.Include(ItemsNavigation);
  }

  public OpportunityWithItemsSpec(OpportunityId opportunityId)
  {
    Query.Where(opportunity => opportunity.Id == opportunityId)
         .Include(ItemsNavigation);
  }

  /// <summary>Nome do campo que e' a navegacao real da colecao de itens.</summary>
  public const string ItemsNavigation = "_items";

  /// <summary>Idem para os documentos, usado pelo detalhe.</summary>
  public const string DocumentsNavigation = "_documents";
}
