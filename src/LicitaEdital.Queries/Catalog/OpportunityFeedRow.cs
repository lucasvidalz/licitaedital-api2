namespace LicitaEdital.Queries.Catalog;

/// <summary>
/// Uma linha do feed: a licitacao mais a compatibilidade **da organizacao que consulta**, ja juntas.
///
/// <para>
/// <b>E' um read model sobre SQL escrito a mao, e nao um `join` em LINQ.</b> O motivo e' uma
/// limitacao concreta do EF Core: ele **nao compara duas colunas com value converter entre si**.
/// `Opportunity.Id` e `OpportunityCompatibility.OpportunityId` sao ambos `OpportunityId` do Vogen,
/// e qualquer forma de escrever a juncao em LINQ — `join ... into`, `SelectMany` com
/// `DefaultIfEmpty`, subconsulta correlacionada — falha em **tempo de execucao** com "could not be
/// translated". A comparacao value object contra parametro funciona; coluna contra coluna, nao.
/// </para>
///
/// <para>
/// O plano ja previa este caminho: "e' a unica listagem do sistema com volume real — candidata
/// natural a query service com SQL proprio". A limitacao do EF so' antecipou a decisao.
/// </para>
///
/// <para>
/// <b>O preco, declarado:</b> os nomes de coluna ficam em texto e nao quebram a compilacao. Em troca
/// sai um unico <c>SELECT</c> que filtra, ordena e pagina no banco — com
/// `ix_opportunity_compatibilities_organization_score` disponivel para o `sort=score`, que e' a
/// ordenacao padrao do feed.
/// </para>
/// </summary>
public sealed class OpportunityFeedRow
{
  public Guid Id { get; private set; }
  public string Title { get; private set; } = string.Empty;
  public string Object { get; private set; } = string.Empty;
  public string BuyerName { get; private set; } = string.Empty;
  public string State { get; private set; } = string.Empty;
  public string City { get; private set; } = string.Empty;
  public string? CityIbgeCode { get; private set; }
  public string ModalityCode { get; private set; } = string.Empty;
  public string ModalityLabel { get; private set; } = string.Empty;
  public string Status { get; private set; } = string.Empty;
  public long? EstimatedValueCents { get; private set; }
  public DateTimeOffset PublishedAt { get; private set; }
  public DateTimeOffset? ProposalDeadline { get; private set; }
  public string OfficialUrl { get; private set; } = string.Empty;
  public string Source { get; private set; } = string.Empty;
  public DateTimeOffset CollectedAt { get; private set; }

  // --- compatibilidade: tudo anulavel, porque `unrated` e' estado real ---

  public int? Score { get; private set; }
  public Guid? OfferingId { get; private set; }

  /// <summary>
  /// Vazias quando nao ha compatibilidade. O <c>COALESCE</c> no SQL garante que o LEFT JOIN sem
  /// par produza array vazio em vez de nulo — o contrato declara as tres listas obrigatorias, e um
  /// nulo aqui viraria `undefined` num campo que o TypeScript promete existir.
  /// </summary>
  public List<string> MatchedTerms { get; private set; } = [];
  public List<string> PositiveReasons { get; private set; } = [];
  public List<string> AttentionPoints { get; private set; } = [];
}
