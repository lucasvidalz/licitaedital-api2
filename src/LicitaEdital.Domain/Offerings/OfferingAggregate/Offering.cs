using LicitaEdital.BuildingBlocks.Domain.Entities;
using LicitaEdital.Domain.Offerings.OfferingAggregate.Events;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Offerings.OfferingAggregate;

/// <summary>
/// O que a empresa vende. E' a **entrada do motor de compatibilidade**: os tres conjuntos de termos
/// e a faixa de valor sao o que casa uma licitacao com o cliente.
///
/// Toda alteracao dispara <see cref="OfferingChangedEvent"/>, porque qualquer score ja calculado
/// para esta organizacao passou a estar errado. Excluir tambem: a exclusao e' logica (herdada de
/// <c>AggregateRoot</c>), e a oferta inativa ainda explica por que uma oportunidade recebeu aquele
/// score no passado.
/// </summary>
public class Offering : AggregateRoot<OfferingId>, ITenantScoped
{
  private readonly List<string> _positiveKeywords = [];
  private readonly List<string> _negativeKeywords = [];
  private readonly List<string> _synonyms = [];
  private readonly List<string> _catalogCodes = [];
  private readonly List<StateCode> _servedRegions = [];

  private Offering(OrganizationId organizationId, OfferingName name, string description,
    SupplyType supplyType)
  {
    OrganizationId = organizationId;
    Name = name;
    Description = description;
    SupplyType = supplyType;
  }

  public OrganizationId OrganizationId { get; private set; }
  public OfferingName Name { get; private set; }
  public string Description { get; private set; }
  public SupplyType SupplyType { get; private set; }

  /// <summary>Termos que aproximam a licitacao desta oferta.</summary>
  public IReadOnlyCollection<string> PositiveKeywords => _positiveKeywords.AsReadOnly();

  /// <summary>Termos que **eliminam** a licitacao, mesmo com termo positivo presente.</summary>
  public IReadOnlyCollection<string> NegativeKeywords => _negativeKeywords.AsReadOnly();

  public IReadOnlyCollection<string> Synonyms => _synonyms.AsReadOnly();

  /// <summary>Codigos CATMAT/CATSER. Casam com `OpportunityLineItem.CatalogCode`.</summary>
  public IReadOnlyCollection<string> CatalogCodes => _catalogCodes.AsReadOnly();

  /// <summary>UFs atendidas. Lista vazia significa "sem restricao", nao "nenhuma".</summary>
  public IReadOnlyCollection<StateCode> ServedRegions => _servedRegions.AsReadOnly();

  /// <summary>Piso de valor em centavos que a empresa aceita disputar. Nulo = sem piso.</summary>
  public long? MinValueCents { get; private set; }

  /// <summary>Teto de valor em centavos. Nulo = sem teto.</summary>
  public long? MaxValueCents { get; private set; }

  Guid ITenantScoped.TenantId => OrganizationId.Value;

  public static Offering Create(OrganizationId organizationId, OfferingName name,
    string description, SupplyType supplyType)
    => new(organizationId, name, description, supplyType);

  public Offering SetTerms(IEnumerable<string> positiveKeywords, IEnumerable<string> negativeKeywords,
    IEnumerable<string> synonyms, IEnumerable<string> catalogCodes)
  {
    Replace(_positiveKeywords, positiveKeywords);
    Replace(_negativeKeywords, negativeKeywords);
    Replace(_synonyms, synonyms);
    Replace(_catalogCodes, catalogCodes);
    RegisterDomainEvent(new OfferingChangedEvent(Id, OrganizationId));
    return this;
  }

  public Offering SetCoverage(IEnumerable<StateCode> servedRegions, long? minValueCents,
    long? maxValueCents)
  {
    _servedRegions.Clear();
    _servedRegions.AddRange(servedRegions.Distinct());
    MinValueCents = minValueCents;
    MaxValueCents = maxValueCents;
    RegisterDomainEvent(new OfferingChangedEvent(Id, OrganizationId));
    return this;
  }

  /// <summary>`PUT /offerings/{id}` substitui a oferta inteira — o request nao tem campo parcial.</summary>
  public Offering Update(OfferingName name, string description, SupplyType supplyType)
  {
    Name = name;
    Description = description;
    SupplyType = supplyType;
    RegisterDomainEvent(new OfferingChangedEvent(Id, OrganizationId));
    return this;
  }

  private static void Replace(List<string> target, IEnumerable<string> values)
  {
    target.Clear();
    target.AddRange(values
      .Select(value => value.Trim())
      .Where(value => value.Length > 0)
      .Distinct(StringComparer.OrdinalIgnoreCase));
  }
}
