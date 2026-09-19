using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.Domain.Catalog.CompatibilityAggregate;
using LicitaEdital.Domain.Catalog.OpportunityAggregate;
using LicitaEdital.Domain.Catalog.OpportunityAggregate.Specifications;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Offerings;

namespace LicitaEdital.Tasks.Catalog;

/// <summary>
/// Recalcula a projecao de compatibilidade. E' o unico lugar que grava
/// <see cref="OpportunityCompatibility"/>.
///
/// <para>
/// <b>Le as ofertas pelo contrato de leitura do modulo Offerings</b>, nunca pelo schema dele: o
/// motor recebe retratos (<see cref="OfferingProfile"/>), e a juncao entre os dois modulos acontece
/// em memoria, com duas consultas independentes. E' o preco de manter a fronteira da spec §4, e e'
/// barato — a lista de ofertas de uma organizacao tem dezenas de linhas.
/// </para>
///
/// <para>
/// <b>Limite conhecido:</b> o recalculo roda **dentro da requisicao** que disparou o evento. Com o
/// volume de hoje (uma organizacao, algumas centenas de licitacoes) isso custa milissegundos. Passa a
/// nao servir quando `licitacoes x organizacoes com oferta` crescer — e o sinal e' o tempo de
/// resposta de `PUT /offerings/{id}` subir. A partir dali isto vira mensagem numa fila, e esta
/// classe continua sendo a que executa, chamada por um consumidor em vez de por um handler de evento.
/// </para>
/// </summary>
public class CompatibilityRecalculator(
  IOfferingsQueryService offerings,
  IReadRepository<Opportunity> opportunities,
  IRepository<OpportunityCompatibility> compatibilities,
  TimeProvider clock,
  ILogger<CompatibilityRecalculator> logger)
{
  private readonly IOfferingsQueryService _offerings = offerings;
  private readonly IReadRepository<Opportunity> _opportunities = opportunities;
  private readonly IRepository<OpportunityCompatibility> _compatibilities = compatibilities;
  private readonly TimeProvider _clock = clock;
  private readonly ILogger<CompatibilityRecalculator> _logger = logger;

  /// <summary>Uma organizacao, todas as licitacoes. Disparado quando a oferta muda.</summary>
  public async Task ForOrganizationAsync(OrganizationId organizationId,
    CancellationToken cancellationToken)
  {
    var profiles = await ProfilesAsync(organizationId, cancellationToken);
    var catalog = await _opportunities.ListAsync(new OpportunityWithItemsSpec(), cancellationToken);

    var stale = await _compatibilities.ListAsync(
      new CompatibilityByOrganizationSpec(organizationId), cancellationToken);

    await ReplaceAsync(organizationId, catalog, profiles, stale, cancellationToken);

    _logger.LogInformation(
      "Compatibilidade recalculada para a organizacao {OrganizationId}: {Offerings} oferta(s), {Opportunities} licitacao(oes)",
      organizationId.Value, profiles.Count, catalog.Count);
  }

  /// <summary>
  /// Uma licitacao, todas as organizacoes com oferta. Disparado quando a coleta traz versao nova —
  /// o objeto que mudou e' o publico, e ele entra no calculo de todo mundo.
  /// </summary>
  public async Task ForOpportunityAsync(OpportunityId opportunityId,
    CancellationToken cancellationToken)
  {
    var opportunity = await _opportunities.FirstOrDefaultAsync(
      new OpportunityWithItemsSpec(opportunityId), cancellationToken);

    if (opportunity is null) return;

    var organizationIds = await _offerings.ListOrganizationsWithOfferingsAsync(cancellationToken);

    var stale = await _compatibilities.ListAsync(
      new CompatibilityByOpportunitySpec(opportunityId), cancellationToken);

    foreach (var organizationId in organizationIds)
    {
      var profiles = await ProfilesAsync(organizationId, cancellationToken);
      await ReplaceAsync(organizationId, [opportunity], profiles,
        [.. stale.Where(row => row.OrganizationId == organizationId)], cancellationToken);
    }

    _logger.LogInformation(
      "Compatibilidade recalculada para a licitacao {OpportunityId} em {Count} organizacao(oes)",
      opportunityId.Value, organizationIds.Count);
  }

  /// <summary>
  /// Apaga as linhas antigas e grava as novas.
  ///
  /// <para>
  /// <b>Apagar e recriar, em vez de atualizar no lugar.</b> `OpportunityCompatibility` foi desenhada
  /// como projecao sem soft delete justamente para isto: uma licitacao que deixou de casar com
  /// qualquer oferta precisa **sumir** da projecao, nao virar uma linha com score nulo — e atualizar
  /// no lugar obrigaria a distinguir os dois casos em toda consulta.
  /// </para>
  /// </summary>
  private async Task ReplaceAsync(OrganizationId organizationId,
    IReadOnlyList<Opportunity> catalog, IReadOnlyList<OfferingProfile> profiles,
    IReadOnlyList<OpportunityCompatibility> stale, CancellationToken cancellationToken)
  {
    if (stale.Count > 0) await _compatibilities.DeleteRangeAsync(stale, cancellationToken);

    // Organizacao sem oferta fica sem linha nenhuma, e a tela le isso como `unrated` — que e' a
    // verdade: nao ha com o que comparar. Gravar zero afirmaria "avaliei e nao serve".
    if (profiles.Count == 0) return;

    var rated = new List<OpportunityCompatibility>();

    foreach (var opportunity in catalog)
    {
      var evaluation = CompatibilityEngine.Evaluate(OpportunityProfile.From(opportunity), profiles);
      if (evaluation is null) continue;

      rated.Add(OpportunityCompatibility
        .Unrated(organizationId, opportunity.Id, CompatibilityEngine.Version, _clock)
        .Rate(evaluation.OfferingId, evaluation.OfferingName, evaluation.Score,
          evaluation.MatchedTerms, evaluation.PositiveReasons, evaluation.AttentionPoints,
          CompatibilityEngine.Version, _clock));
    }

    if (rated.Count > 0) await _compatibilities.AddRangeAsync(rated, cancellationToken);
  }

  private async Task<IReadOnlyList<OfferingProfile>> ProfilesAsync(OrganizationId organizationId,
    CancellationToken cancellationToken)
  {
    var offerings = await _offerings.ListAsync(organizationId, cancellationToken);

    return
    [
      .. offerings.Select(offering => new OfferingProfile(
        OfferingId.From(offering.Id),
        offering.Name,
        offering.PositiveKeywords,
        offering.NegativeKeywords,
        offering.Synonyms,
        offering.CatalogCodes,
        [.. offering.ServedRegions.Select(StateCode.Parse)],
        offering.MinValueCents,
        offering.MaxValueCents))
    ];
  }
}
