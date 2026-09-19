using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Engagement;

namespace LicitaEdital.Application.Engagement.Settings.Get;

/// <summary>
/// **Nunca responde 404.** Organizacao sem preferencia gravada recebe o padrao do dominio — a tela de
/// configuracoes nao tem o estado "ainda nao cadastrado" que `/company-profile` tem.
/// </summary>
public class GetSettingsHandler(IExecutionContext execution, IAlertPreferencesQueryService preferences)
  : IQueryHandler<GetSettingsQuery, Result<SettingsDto>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IAlertPreferencesQueryService _preferences = preferences;

  public async ValueTask<Result<SettingsDto>> Handle(GetSettingsQuery query,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<SettingsDto>.Unauthorized();

    return await _preferences.GetAsync(OrganizationId.From(tenantId), cancellationToken);
  }
}
