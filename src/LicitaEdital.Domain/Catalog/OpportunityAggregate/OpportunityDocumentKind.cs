namespace LicitaEdital.Domain.Catalog.OpportunityAggregate;

/// <summary>Espelha `OpportunityDocumentKind` (`opportunity-detail-api.model.ts:33-34`).</summary>
public sealed class OpportunityDocumentKind : SmartEnum<OpportunityDocumentKind, string>
{
  public static readonly OpportunityDocumentKind Edital = new(nameof(Edital), "edital");
  public static readonly OpportunityDocumentKind TermoReferencia = new(nameof(TermoReferencia), "termo_referencia");
  public static readonly OpportunityDocumentKind Anexo = new(nameof(Anexo), "anexo");
  public static readonly OpportunityDocumentKind Retificacao = new(nameof(Retificacao), "retificacao");
  public static readonly OpportunityDocumentKind Complementar = new(nameof(Complementar), "complementar");

  private OpportunityDocumentKind(string name, string value) : base(name, value) { }
}
