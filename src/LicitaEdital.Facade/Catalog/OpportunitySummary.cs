using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Facade.Catalog;

/// <summary>
/// Recorte de licitacao que o Catalog publica para os outros modulos.
///
/// <para>
/// <b>Parece igual ao DTO da API de listagem, e e' de proposito que sejam tipos diferentes.</b> Este
/// e' o contrato **entre modulos**; aquele e' o contrato **com o cliente**. Compartilhar um tipo so
/// faria toda mudanca de tela obrigar uma mudanca no acordo entre Catalog e Engagement — e o
/// inverso tambem. Hoje os dois coincidem; o dia em que a tela pedir um campo novo, so um dos dois
/// muda.
/// </para>
///
/// Tipos primitivos e ids opacos, nunca entidade: devolver <c>Opportunity</c> daria ao outro modulo
/// acesso aos metodos que mudam o agregado.
/// </summary>
public sealed record OpportunitySummary(
  OpportunityId Id,
  string Title,
  string Object,
  string BuyerName,
  StateCode State,
  string City,
  string? CityIbgeCode,
  string ModalityCode,
  string ModalityLabel,
  string Status,
  long? EstimatedValueCents,
  DateTimeOffset PublishedAt,
  DateTimeOffset? ProposalDeadline,
  string OfficialUrl,
  string Source,
  DateTimeOffset CollectedAt,
  CompatibilitySummary Compatibility);

/// <summary>Compatibilidade da licitacao com as ofertas da organizacao que perguntou.</summary>
public sealed record CompatibilitySummary(
  int? Score,
  OfferingId? OfferingId,
  IReadOnlyList<string> MatchedTerms,
  IReadOnlyList<string> PositiveReasons,
  IReadOnlyList<string> AttentionPoints)
{
  /// <summary>Organizacao sem oferta cadastrada: `unrated` na tela, sem nota nenhuma.</summary>
  public static CompatibilitySummary Unrated => new(null, null, [], [], []);
}
