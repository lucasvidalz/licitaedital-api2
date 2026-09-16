using LicitaEdital.Application.Companies.Profile.Get;
using LicitaEdital.BuildingBlocks.Auth.Authorization;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LicitaEdital.Api.CompanyProfile;

/// <summary>
/// Cadastro da empresa da organizacao da sessao.
///
/// <para>
/// <b>404 e' estado normal aqui</b> (`AD-032`), nao falha: empresa que ainda nao preencheu o
/// formulario. O `CompanyProfileStore` do Angular trata esse 404 como sucesso com `profile: null` e
/// abre o formulario vazio — por isso ele **nao** passa por `skipGlobalErrorHandling`, e por isso
/// este caminho nao deve ser logado como erro.
/// </para>
/// </summary>
public class Get(IMediator mediator)
  : EndpointWithoutRequest<Results<Ok<CompanyProfileResponse>, NotFound, ProblemHttpResult>>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    base.Get(CompanyProfileRoutes.Get);
    // Sem permissao nomeada: o cadastro da propria empresa e' do cliente, e quem o autoriza e' a
    // area. As permissoes que existem hoje sao todas de gerenciamento (ver `IdentitySeeder`).
    Policies(AuthorizationPolicies.Area(UserArea.Codes.Client));
    Tags("CompanyProfile");
    Summary(s =>
    {
      s.Summary = "Cadastro da empresa";
      s.Responses[200] = "Perfil cadastrado";
      s.Responses[404] = "Ainda não há cadastro — estado normal, a tela abre o formulário vazio";
    });
  }

  public override async Task<Results<Ok<CompanyProfileResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetCompanyProfileQuery(), cancellationToken);

    return result.ToGetResult(CompanyProfileResponse.From);
  }
}
