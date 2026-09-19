using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Engagement;

namespace LicitaEdital.Application.Engagement.Subscription.Get;

/// <summary>
/// O plano vigente. **404 quando nao ha assinatura registrada**, e nao um plano padrao inventado:
/// em que plano a empresa esta e' fato comercial, e afirma-lo sem ninguem ter registrado seria o
/// servidor criando contrato. O `SubscriptionStore` do Angular ja trata 404 como dado ausente.
/// </summary>
public class GetSubscriptionHandler(IExecutionContext execution, ISubscriptionQueryService subscriptions)
  : IQueryHandler<GetSubscriptionQuery, Result<string>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly ISubscriptionQueryService _subscriptions = subscriptions;

  public async ValueTask<Result<string>> Handle(GetSubscriptionQuery query,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<string>.Unauthorized();

    var planId = await _subscriptions.GetPlanIdAsync(OrganizationId.From(tenantId), cancellationToken);

    return planId is null ? Result<string>.NotFound() : planId;
  }
}
