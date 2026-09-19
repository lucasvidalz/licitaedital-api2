namespace LicitaEdital.Queries.Contracts.Engagement;

/// <summary>
/// Preferencias de alerta da organizacao.
///
/// <b>Nunca devolve nulo.</b> Organizacao sem linha gravada recebe o padrao do dominio — a tela de
/// configuracoes nao tem estado "ainda nao cadastrado", diferente de `/company-profile`.
/// </summary>
public interface IAlertPreferencesQueryService
{
  Task<SettingsDto> GetAsync(OrganizationId organizationId,
    CancellationToken cancellationToken = default);
}

/// <summary>
/// Plano vigente.
///
/// <b>Devolve nulo quando nao ha assinatura registrada</b>, e o endpoint traduz para 404. Nao
/// inventa um plano padrao: em que plano a empresa esta e' fato comercial, e afirmar "inicial" sem
/// ninguem ter registrado seria o servidor inventando contrato. O `SubscriptionStore` do Angular ja
/// trata 404 como `subscription: null` (`AD-032`).
/// </summary>
public interface ISubscriptionQueryService
{
  Task<string?> GetPlanIdAsync(OrganizationId organizationId,
    CancellationToken cancellationToken = default);
}
