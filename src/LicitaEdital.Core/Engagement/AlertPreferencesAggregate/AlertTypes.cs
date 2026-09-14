namespace LicitaEdital.Core.Engagement.AlertPreferencesAggregate;

/// <summary>
/// Quais avisos a organizacao quer receber. Tipo proprio (mapeado como owned) em vez de tres
/// colunas soltas: os tres so fazem sentido juntos, e o contrato os envia aninhados em
/// `alertTypes`.
/// </summary>
public record AlertTypes(bool NewCompatibleOpportunity, bool ApproachingDeadline, bool DailySummary)
{
  public static AlertTypes Default => new(NewCompatibleOpportunity: true, ApproachingDeadline: true, DailySummary: false);
}
