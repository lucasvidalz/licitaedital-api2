using LicitaEdital.Application.Companies.Profile.Save;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.CompanyProfile;

/// <summary>
/// Grava o cadastro da empresa. **`PUT` e upsert idempotente**: o mesmo corpo enviado duas vezes
/// deixa o mesmo estado, e o cliente nao precisa saber se ja existia cadastro — por isso nao ha
/// `POST /company-profile`.
/// </summary>
public class Save(IMediator mediator)
  : Endpoint<CompanyProfileRequest,
             Results<Ok<CompanyProfileResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Put(CompanyProfileRoutes.Save);
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("CompanyProfile");
    Summary(s =>
    {
      s.Summary = "Cria ou substitui o cadastro da empresa";
      s.Responses[200] = "Perfil gravado, no mesmo formato de GET /company-profile";
      s.Responses[400] = "Campo inválido, ou CNPJ já cadastrado em outra conta";
    });
  }

  public override async Task<Results<Ok<CompanyProfileResponse>, NotFound, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CompanyProfileRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new SaveCompanyProfileCommand(request.CompanyName, request.Cnpj, request.City, request.State,
        request.BusinessArea),
      cancellationToken);

    return result.ToUpdateResult(CompanyProfileResponse.From);
  }
}
