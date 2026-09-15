namespace LicitaEdital.Domain.Engagement.AlertPreferencesAggregate;

/// <summary>Espelha `AlertFrequency` (`settings.model.ts:3`).</summary>
public sealed class AlertFrequency : SmartEnum<AlertFrequency, string>
{
  public static readonly AlertFrequency Immediate = new(nameof(Immediate), "immediate");
  public static readonly AlertFrequency DailyDigest = new(nameof(DailyDigest), "daily-digest");
  public static readonly AlertFrequency WeeklyDigest = new(nameof(WeeklyDigest), "weekly-digest");

  private AlertFrequency(string name, string value) : base(name, value) { }
}
