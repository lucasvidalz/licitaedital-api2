using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Collections.CoverageSettingsAggregate;

/// <summary>
/// Ate onde o radar cobre: `GET`/`PUT /gfe/settings`. E' configuracao **da plataforma**, editada
/// pela area de gerenciamento, e por isso mora em Collections — ela governa o que o coletor busca,
/// nao a preferencia de um cliente (isso e'
/// <see cref="Engagement.AlertPreferencesAggregate.AlertPreferences"/>).
///
/// Singleton: existe uma linha so. O id existe porque o EF Core precisa de chave, nao porque haja
/// mais de uma cobertura.
/// </summary>
public class CoverageSettings : EntityBase<CoverageSettings, CoverageSettingsId>, IAggregateRoot
{
  private readonly List<StateCode> _attendedStates = [];

  private CoverageSettings(ValueRange valueRange)
  {
    ValueRange = valueRange;
  }

  /// <summary>UFs cobertas pela coleta. Vazio significa "todas", nao "nenhuma".</summary>
  public IReadOnlyCollection<StateCode> AttendedStates => _attendedStates.AsReadOnly();

  public ValueRange ValueRange { get; private set; }
  public DateTimeOffset UpdatedAt { get; private set; }

  public static CoverageSettings CreateDefault(TimeProvider clock)
      => new(ValueRange.Unset) { Id = CoverageSettingsId.New(), UpdatedAt = clock.GetUtcNow() };

  public CoverageSettings Update(IEnumerable<StateCode> attendedStates, ValueRange valueRange,
    TimeProvider clock)
  {
    _attendedStates.Clear();
    _attendedStates.AddRange(attendedStates.Distinct());
    ValueRange = valueRange;
    UpdatedAt = clock.GetUtcNow();
    return this;
  }
}
