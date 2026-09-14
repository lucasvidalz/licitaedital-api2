using LicitaEdital.BuildingBlocks.Domain.Entities;
using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Engagement.AlertPreferencesAggregate;

/// <summary>
/// O que `GET /settings` e `PUT /settings` leem e gravam. Singular por organizacao.
///
/// **Preferencia de exibicao nao entra aqui.** Ordenacao, densidade e tema sao escolha de
/// visualizacao e moram no navegador (AD-044 do frontend); o que chega ao servidor e' so o que muda
/// o comportamento do produto — quais alertas disparar, com que frequencia e sobre qual recorte.
/// </summary>
public class AlertPreferences : AggregateRoot<AlertPreferencesId>, ITenantScoped
{
  private readonly List<StateCode> _filterStates = [];
  private readonly List<string> _filterModalities = [];

  private AlertPreferences(OrganizationId organizationId, AlertTypes types, AlertFrequency frequency,
    ValueRange filterValueRange)
  {
    OrganizationId = organizationId;
    Types = types;
    Frequency = frequency;
    FilterValueRange = filterValueRange;
  }

  public OrganizationId OrganizationId { get; private set; }

  /// <summary>Quais avisos disparar. Owned type — tres colunas na mesma tabela.</summary>
  public AlertTypes Types { get; private set; }

  public AlertFrequency Frequency { get; private set; }

  /// <summary>Faixa de valor do recorte padrao, como chave simbolica.</summary>
  public ValueRange FilterValueRange { get; private set; }

  /// <summary>Leitura agrupada dos tres campos de filtro, na forma que o contrato usa.</summary>
  public AlertFilter Filter => new(_filterStates.AsReadOnly(), _filterModalities.AsReadOnly(), FilterValueRange);

  Guid ITenantScoped.TenantId => OrganizationId.Value;

  /// <summary>
  /// Organizacao sem preferencia gravada recebe este padrao — `GET /settings` nunca responde 404,
  /// diferente de `/company-profile`. A tela de configuracoes nao tem estado "ainda nao cadastrado".
  /// </summary>
  public static AlertPreferences CreateDefault(OrganizationId organizationId)
      => new(organizationId, AlertTypes.Default, AlertFrequency.DailyDigest, ValueRange.Unset);

  public AlertPreferences Update(AlertTypes types, AlertFrequency frequency, AlertFilter filter)
  {
    Types = types;
    Frequency = frequency;

    _filterStates.Clear();
    _filterStates.AddRange(filter.States.Distinct());
    _filterModalities.Clear();
    _filterModalities.AddRange(filter.Modalities.Distinct(StringComparer.OrdinalIgnoreCase));
    FilterValueRange = filter.ValueRange;

    return this;
  }
}
