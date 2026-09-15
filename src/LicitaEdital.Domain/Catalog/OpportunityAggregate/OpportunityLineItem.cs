using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Domain.Catalog.OpportunityAggregate;

[ValueObject<Guid>]
public readonly partial struct OpportunityLineItemId : IGuidId<OpportunityLineItemId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("OpportunityLineItemId nao pode ser vazio.");
}

/// <summary>
/// Item publicado pela licitacao. So aparece no detalhe (`GET /opportunities/{id}`), nunca na
/// listagem — carregar item em pagina de 20 oportunidades multiplicaria a consulta sem uso na tela.
///
/// Herda de <c>Entity</c>, e nao de <c>AggregateRoot</c>: e' filho do agregado, sem repositorio
/// proprio, sem auditoria e **sem soft delete** — item que a fonte deixou de publicar some de
/// verdade, porque nao tem historico proprio a preservar.
///
/// Valor unitario e total sao **centavos**, e podem faltar: orgao nem sempre publica estimativa por
/// item.
/// </summary>
public class OpportunityLineItem : Entity<OpportunityLineItemId>
{
  private OpportunityLineItem(int number, string description, decimal quantity, string unit,
    long? unitValueCents, long? totalValueCents, string? catalogCode)
  {
    Number = number;
    Description = description;
    Quantity = quantity;
    Unit = unit;
    UnitValueCents = unitValueCents;
    TotalValueCents = totalValueCents;
    CatalogCode = catalogCode;
  }

  /// <summary>Numero do item no edital. Unico dentro da oportunidade.</summary>
  public int Number { get; private set; }

  public string Description { get; private set; }
  public decimal Quantity { get; private set; }
  public string Unit { get; private set; }
  public long? UnitValueCents { get; private set; }
  public long? TotalValueCents { get; private set; }

  /// <summary>Codigo CATMAT/CATSER quando a fonte publica.</summary>
  public string? CatalogCode { get; private set; }

  public static OpportunityLineItem Create(int number, string description, decimal quantity,
    string unit, long? unitValueCents, long? totalValueCents, string? catalogCode)
    => new(number, description, quantity, unit, unitValueCents, totalValueCents, catalogCode);
}
