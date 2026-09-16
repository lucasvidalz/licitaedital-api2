using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Companies;

namespace LicitaEdital.Application.Companies.Profile.Get;

/// <summary>
/// <b>`NotFound` aqui nao e' erro.</b> Organizacao que ainda nao cadastrou a empresa e' o primeiro
/// estado do ciclo de vida: o endpoint responde 404 e a tela abre formulario vazio (`AD-032`).
/// Registrar isso como falha encheria o log de ruido no dia de maior cadastro do produto.
/// </summary>
public class GetCompanyProfileHandler(IExecutionContext execution, ICompanyProfileQueryService profiles)
  : IQueryHandler<GetCompanyProfileQuery, Result<CompanyProfileDto>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly ICompanyProfileQueryService _profiles = profiles;

  public async ValueTask<Result<CompanyProfileDto>> Handle(GetCompanyProfileQuery query,
    CancellationToken cancellationToken)
  {
    if (_execution.TenantId is not { } tenantId) return Result<CompanyProfileDto>.Unauthorized();

    var profile = await _profiles.FindAsync(OrganizationId.From(tenantId), cancellationToken);

    return profile is null ? Result<CompanyProfileDto>.NotFound() : profile;
  }
}
