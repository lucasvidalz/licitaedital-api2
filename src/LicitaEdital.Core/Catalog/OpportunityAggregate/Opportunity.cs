using LicitaEdital.Core.Catalog.OpportunityAggregate.Events;
using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Catalog.OpportunityAggregate;

/// <summary>
/// Licitacao publicada, como a fonte a divulgou. **Nao tem `OrganizationId`**: e' dado publico,
/// compartilhado por todos os clientes — a unica excecao a regra D-02, e deliberada. O que e'
/// privado de cada organizacao e' a compatibilidade
/// (<see cref="CompatibilityAggregate.OpportunityCompatibility"/>) e o salvamento
/// (modulo Engagement), que sao entidades separadas justamente por isso.
///
/// Identidade de negocio e' o par <see cref="Source"/> + <see cref="ExternalReference"/>, unico:
/// e' o que torna a coleta idempotente — reprocessar a mesma execucao atualiza, nunca duplica.
/// </summary>
public class Opportunity : EntityBase<Opportunity, OpportunityId>, IAggregateRoot
{
  private readonly List<OpportunityLineItem> _items = [];
  private readonly List<OpportunityDocument> _documents = [];

  private Opportunity(string source, string externalReference, string title, string @object,
    string buyerName, StateCode state, string city, Modality modality, OpportunityStatus status,
    string officialUrl)
  {
    Source = source;
    ExternalReference = externalReference;
    Title = title;
    Object = @object;
    BuyerName = buyerName;
    State = state;
    City = city;
    Modality = modality;
    Status = status;
    OfficialUrl = officialUrl;
  }

  // --- identidade na fonte ---

  /// <summary>Fonte que publicou (PNCP, Compras.gov.br...). Compoe a chave natural.</summary>
  public string Source { get; private set; }

  /// <summary>Identificador da licitacao na fonte. Compoe a chave natural.</summary>
  public string ExternalReference { get; private set; }

  // --- descricao ---
  public string Title { get; private set; }
  public string Object { get; private set; }
  public string BuyerName { get; private set; }

  /// <summary>Numero do contrato/processo. So o detalhe expoe (`opportunity-detail-api.model.ts:8`).</summary>
  public string? ContractNumber { get; private set; }

  // --- localizacao ---
  public StateCode State { get; private set; }
  public string City { get; private set; }

  /// <summary>Codigo IBGE do municipio, quando a fonte publica.</summary>
  public string? CityIbgeCode { get; private set; }

  // --- classificacao e prazos ---
  public Modality Modality { get; private set; }
  public OpportunityStatus Status { get; private set; }

  /// <summary>Valor estimado em **centavos**. Nulo quando o orgao nao publica.</summary>
  public long? EstimatedValueCents { get; private set; }

  public DateTimeOffset PublishedAt { get; private set; }

  /// <summary>Limite para envio de proposta. Nulo e' estado real — ha edital sem prazo publicado.</summary>
  public DateTimeOffset? ProposalDeadline { get; private set; }

  public string OfficialUrl { get; private set; }

  /// <summary>Quando a coleta leu esta versao. E' o que a tela mostra como "ultimo dado obtido".</summary>
  public DateTimeOffset CollectedAt { get; private set; }

  public DateTimeOffset CreatedAt { get; private set; }
  public DateTimeOffset UpdatedAt { get; private set; }

  public IReadOnlyCollection<OpportunityLineItem> Items => _items.AsReadOnly();
  public IReadOnlyCollection<OpportunityDocument> Documents => _documents.AsReadOnly();

  public static Opportunity Publish(string source, string externalReference, string title,
    string @object, string buyerName, StateCode state, string city, Modality modality,
    OpportunityStatus status, string officialUrl, DateTimeOffset publishedAt,
    DateTimeOffset collectedAt, TimeProvider clock)
  {
    var now = clock.GetUtcNow();
    return new Opportunity(source, externalReference, title, @object, buyerName, state, city,
      modality, status, officialUrl)
    {
      Id = OpportunityId.New(),
      PublishedAt = publishedAt,
      CollectedAt = collectedAt,
      CreatedAt = now,
      UpdatedAt = now
    };
  }

  public Opportunity WithLocationDetail(string? cityIbgeCode, string? contractNumber)
  {
    CityIbgeCode = cityIbgeCode;
    ContractNumber = contractNumber;
    return this;
  }

  public Opportunity WithEstimate(long? estimatedValueCents, DateTimeOffset? proposalDeadline)
  {
    EstimatedValueCents = estimatedValueCents;
    ProposalDeadline = proposalDeadline;
    return this;
  }

  /// <summary>
  /// Aplica o que a coleta leu. Dispara <see cref="OpportunityRefreshedEvent"/> para que a
  /// compatibilidade seja recalculada — score sobre objeto desatualizado e' pior que score ausente.
  /// </summary>
  public Opportunity Refresh(OpportunityStatus status, long? estimatedValueCents,
    DateTimeOffset? proposalDeadline, DateTimeOffset collectedAt, TimeProvider clock)
  {
    Status = status;
    EstimatedValueCents = estimatedValueCents;
    ProposalDeadline = proposalDeadline;
    CollectedAt = collectedAt;
    UpdatedAt = clock.GetUtcNow();
    RegisterDomainEvent(new OpportunityRefreshedEvent(Id));
    return this;
  }

  public Opportunity ReplaceItems(IEnumerable<OpportunityLineItem> items)
  {
    _items.Clear();
    _items.AddRange(items);
    return this;
  }

  public Opportunity ReplaceDocuments(IEnumerable<OpportunityDocument> documents)
  {
    _documents.Clear();
    _documents.AddRange(documents);
    return this;
  }
}
