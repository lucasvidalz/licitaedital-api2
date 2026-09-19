using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Domain.Engagement.SavedOpportunityAggregate;
using LicitaEdital.Domain.Engagement.SavedOpportunityAggregate.Specifications;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Application.Engagement.SavedOpportunities.Unsave;

/// <summary>
/// Remove o salvamento. O id da rota e' o da **licitacao**, nao o do registro — e' o que o contrato
/// define, e e' o unico id que a tela tem em maos.
///
/// <para>
/// <b>Idempotente</b>: dessalvar o que nao estava salvo responde 204. E' um alternador, e devolver
/// 404 faria a tela mostrar erro por uma operacao que atingiu o resultado desejado.
/// </para>
/// </summary>
public class UnsaveOpportunityHandler(
  IExecutionContext execution,
  IRepository<SavedOpportunity> saved)
  : ICommandHandler<UnsaveOpportunityCommand, Result>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IRepository<SavedOpportunity> _saved = saved;

  public async ValueTask<Result> Handle(UnsaveOpportunityCommand command,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result.Unauthorized();

    var existing = await _saved.FirstOrDefaultAsync(
      new SavedOpportunitySpec(OrganizationId.From(tenantId), command.OpportunityId),
      cancellationToken);

    if (existing is not null) await _saved.DeleteAsync(existing, cancellationToken);

    return Result.Success();
  }
}
