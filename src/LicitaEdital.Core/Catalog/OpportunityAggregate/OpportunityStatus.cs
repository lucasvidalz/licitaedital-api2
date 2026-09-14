namespace LicitaEdital.Core.Catalog.OpportunityAggregate;

/// <summary>
/// Situacao publicada pela fonte. Os 4 valores sao os de `OpportunityStatus`
/// (`opportunity-api.model.ts:25`) e viajam literalmente no contrato.
/// </summary>
public sealed class OpportunityStatus : SmartEnum<OpportunityStatus, string>
{
  public static readonly OpportunityStatus ReceivingProposals = new(nameof(ReceivingProposals), "receiving_proposals");
  public static readonly OpportunityStatus Closed = new(nameof(Closed), "closed");
  public static readonly OpportunityStatus Cancelled = new(nameof(Cancelled), "cancelled");
  public static readonly OpportunityStatus Reopened = new(nameof(Reopened), "reopened");

  private OpportunityStatus(string name, string value) : base(name, value) { }
}
