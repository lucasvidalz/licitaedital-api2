using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Catalog.CompatibilityAggregate;

/// <summary>
/// Quanto uma licitacao casa com o que a organizacao vende. **Projecao local do modulo Catalog**,
/// alimentada por evento de Offerings — permitida pela spec §4 ("projecoes locais sao permitidas se
/// tiverem proprietario, versao e estrategia de atualizacao").
///
/// Mora em Catalog, e nao em Offerings, por uma razao de consulta: `GET /opportunities?sort=score`
/// filtra por estado, modalidade e valor **e** ordena por score na mesma pagina. Com a nota em
/// outro schema, isso exigiria join entre modulos — proibido — ou duas consultas que nao paginam
/// juntas.
///
/// Uma linha por (organizacao, licitacao): guardamos **a melhor** oferta, que e' o que o contrato
/// expoe. <see cref="EngineVersion"/> e <see cref="CalculatedAt"/> sao a estrategia de atualizacao:
/// mudou a versao do motor, a linha esta obsoleta e entra na fila de recalculo.
/// </summary>
public class OpportunityCompatibility
  : EntityBase<OpportunityCompatibility, OpportunityCompatibilityId>, IAggregateRoot
{
  private readonly List<string> _matchedTerms = [];
  private readonly List<string> _positiveReasons = [];
  private readonly List<string> _attentionPoints = [];

  private OpportunityCompatibility(OrganizationId organizationId, OpportunityId opportunityId,
    string engineVersion)
  {
    OrganizationId = organizationId;
    OpportunityId = opportunityId;
    EngineVersion = engineVersion;
  }

  public OrganizationId OrganizationId { get; private set; }
  public OpportunityId OpportunityId { get; private set; }

  /// <summary>Oferta que produziu a melhor nota. Nulo quando nenhuma oferta casou.</summary>
  public OfferingId? OfferingId { get; private set; }

  /// <summary>
  /// Nome da oferta no momento do calculo. Copia deliberada: o detalhe expoe `offeringName`
  /// (`opportunity-detail-api.model.ts:50`) e ler o nome vivo exigiria alcancar o schema de
  /// Offerings a cada listagem.
  /// </summary>
  public string? OfferingName { get; private set; }

  /// <summary>Nulo e' estado real: `unrated`, quando a organizacao ainda nao cadastrou oferta.</summary>
  public CompatibilityScore? Score { get; private set; }

  public IReadOnlyCollection<string> MatchedTerms => _matchedTerms.AsReadOnly();
  public IReadOnlyCollection<string> PositiveReasons => _positiveReasons.AsReadOnly();
  public IReadOnlyCollection<string> AttentionPoints => _attentionPoints.AsReadOnly();

  /// <summary>Versao do motor de compatibilidade que produziu esta linha.</summary>
  public string EngineVersion { get; private set; }

  public DateTimeOffset CalculatedAt { get; private set; }

  public static OpportunityCompatibility Unrated(OrganizationId organizationId,
    OpportunityId opportunityId, string engineVersion, TimeProvider clock)
    => new(organizationId, opportunityId, engineVersion)
    {
      Id = OpportunityCompatibilityId.New(),
      CalculatedAt = clock.GetUtcNow()
    };

  public OpportunityCompatibility Rate(OfferingId offeringId, string offeringName,
    CompatibilityScore score, IEnumerable<string> matchedTerms, IEnumerable<string> positiveReasons,
    IEnumerable<string> attentionPoints, string engineVersion, TimeProvider clock)
  {
    OfferingId = offeringId;
    OfferingName = offeringName;
    Score = score;
    EngineVersion = engineVersion;
    CalculatedAt = clock.GetUtcNow();

    _matchedTerms.Clear();
    _matchedTerms.AddRange(matchedTerms);
    _positiveReasons.Clear();
    _positiveReasons.AddRange(positiveReasons);
    _attentionPoints.Clear();
    _attentionPoints.AddRange(attentionPoints);

    return this;
  }

  /// <summary>Obsoleta quando o motor evoluiu depois do ultimo calculo desta linha.</summary>
  public bool IsStale(string currentEngineVersion) => EngineVersion != currentEngineVersion;
}
