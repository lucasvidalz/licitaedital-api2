using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Domain.Catalog.OpportunityAggregate;

[ValueObject<Guid>]
public readonly partial struct OpportunityDocumentId : IGuidId<OpportunityDocumentId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("OpportunityDocumentId nao pode ser vazio.");
}

/// <summary>
/// Edital e anexos publicados pela fonte. Guardamos **a URL oficial**, nunca os bytes: o documento e'
/// publico, pertence ao orgao, e baixa-lo para o nosso storage criaria copia com validade propria —
/// retificacao publicada depois tornaria a copia mentirosa em silencio.
///
/// Nao confundir com o cofre documental (modulo Documents, Fase 7), que guarda documento **da
/// empresa**, em versao imutavel e com bytes sob nosso controle.
/// </summary>
public class OpportunityDocument : Entity<OpportunityDocumentId>
{
  private OpportunityDocument(OpportunityDocumentKind kind, string label, string url,
    DateTimeOffset? publishedAt)
  {
    Kind = kind;
    Label = label;
    Url = url;
    PublishedAt = publishedAt;
  }

  public OpportunityDocumentKind Kind { get; private set; }
  public string Label { get; private set; }

  /// <summary>URL oficial. Somente HTTPS, host validado (spec §16) — o servidor nao segue qualquer URL.</summary>
  public string Url { get; private set; }

  public DateTimeOffset? PublishedAt { get; private set; }

  public static OpportunityDocument Create(OpportunityDocumentKind kind, string label, string url,
    DateTimeOffset? publishedAt)
    => new(kind, label, url, publishedAt);
}
