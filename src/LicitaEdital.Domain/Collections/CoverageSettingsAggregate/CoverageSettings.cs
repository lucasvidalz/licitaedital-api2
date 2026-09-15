using LicitaEdital.BuildingBlocks.Domain.Entities;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Collections.CoverageSettingsAggregate;

/// <summary>
/// Ate onde o radar cobre: `GET`/`PUT /gfe/settings`. E' configuracao **da plataforma**, editada
/// pela area de gerenciamento, e por isso mora em Collections — ela governa o que o coletor busca,
/// nao a preferencia de um cliente (isso e'
/// <see cref="LicitaEdital.Domain.Engagement.AlertPreferencesAggregate.AlertPreferences"/>).
///
/// Singleton, sem tenant e sem soft delete. O id existe porque o EF Core precisa de chave, nao
/// porque haja mais de uma cobertura.
/// </summary>
public class CoverageSettings : AuditableEntity<CoverageSettingsId>, IAggregateRoot
{
  private readonly List<StateCode> _attendedStates = [];

  private CoverageSettings(ValueRange valueRange)
  {
    ValueRange = valueRange;
  }

  /// <summary>UFs cobertas pela coleta. Vazio significa "todas", nao "nenhuma".</summary>
  public IReadOnlyCollection<StateCode> AttendedStates => _attendedStates.AsReadOnly();

  public ValueRange ValueRange { get; private set; }

  public static CoverageSettings CreateDefault() => new(ValueRange.Unset);

  public CoverageSettings Update(IEnumerable<StateCode> attendedStates, ValueRange valueRange)
  {
    _attendedStates.Clear();
    _attendedStates.AddRange(attendedStates.Distinct());
    ValueRange = valueRange;
    return this;
  }
}
