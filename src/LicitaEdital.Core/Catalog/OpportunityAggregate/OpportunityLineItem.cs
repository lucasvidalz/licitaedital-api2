using Vogen;

namespace LicitaEdital.Core.Catalog.OpportunityAggregate;

[ValueObject<Guid>]
public readonly partial struct OpportunityLineItemId
{
  public static OpportunityLineItemId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("OpportunityLineItemId nao pode ser vazio.");
}

/// <summary>
/// Item publicado pela licitacao. So aparece no detalhe (`GET /opportunities/{id}`), nunca na
/// listagem — carregar item em pagina de 20 oportunidades multiplicaria a consulta sem uso na tela.
///
/// Pertence ao agregado <see cref="Opportunity"/>: nao tem repositorio proprio e nao se altera
/// isoladamente. Valor unitario e total sao **centavos**, e podem faltar — orgao nem sempre publica
/// estimativa por item.
/// </summary>
public class OpportunityLineItem : EntityBase<OpportunityLineItem, OpportunityLineItemId>
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
    => new(number, description, quantity, unit, unitValueCents, totalValueCents, catalogCode)
    {
      Id = OpportunityLineItemId.New()
    };
}
