using LicitaEdital.BuildingBlocks.Domain.Entities;
using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Catalog.CompatibilityAggregate;

/// <summary>
/// Quanto uma licitacao casa com o que a organizacao vende. **Projecao local do modulo Catalog**,
/// alimentada por evento de Offerings — permitida pela spec §4 ("projecoes locais sao permitidas se
/// tiverem proprietario, versao e estrategia de atualizacao").
///
/// Mora em Catalog, e nao em Offerings, por uma razao de consulta: `GET /opportunities?sort=score`
/// filtra por estado, modalidade e valor **e** ordena por score na mesma pagina. Com a nota em outro
/// schema, isso exigiria join entre modulos — proibido — ou duas consultas que nao paginam juntas.
///
/// **Herda de <c>AuditableEntity</c>, nao de <c>AggregateRoot</c>: projecao nao tem soft delete.**
/// Linha obsoleta deve sumir de verdade no recalculo; mantida como inativa, ela apareceria em
/// qualquer consulta que esquecesse o filtro e competiria pela unicidade
/// (organizacao, licitacao) com a linha nova.
/// </summary>
public class OpportunityCompatibility : AuditableEntity<OpportunityCompatibilityId>,
  IAggregateRoot, ITenantScoped
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

  Guid ITenantScoped.TenantId => OrganizationId.Value;

  public static OpportunityCompatibility Unrated(OrganizationId organizationId,
    OpportunityId opportunityId, string engineVersion, TimeProvider clock)
    => new(organizationId, opportunityId, engineVersion) { CalculatedAt = clock.GetUtcNow() };

  public OpportunityCompatibility Rate(OfferingId offeringId, string offeringName,
    CompatibilityScore score, IEnumerable<string> matchedTerms, IEnumerable<string> positiveReasons,
    IEnumerable<string> attentionPoints, string engineVersion, TimeProvider clock)
  {
    OfferingId = offeringId;
    OfferingName = offeringName;
    Score = score;
    EngineVersion = engineVersion;
    CalculatedAt = clock.GetUtcNow();

    Replace(_matchedTerms, matchedTerms);
    Replace(_positiveReasons, positiveReasons);
    Replace(_attentionPoints, attentionPoints);

    return this;
  }

  /// <summary>Obsoleta quando o motor evoluiu depois do ultimo calculo desta linha.</summary>
  public bool IsStale(string currentEngineVersion) => EngineVersion != currentEngineVersion;

  private static void Replace(List<string> target, IEnumerable<string> values)
  {
    target.Clear();
    target.AddRange(values);
  }
}
