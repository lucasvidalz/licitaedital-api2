using LicitaEdital.BuildingBlocks.Domain.Execution;
using LicitaEdital.Core.Shared;

namespace LicitaEdital.UseCases.Identity.Me;

/// <summary>
/// Resolve a sessao no boot do frontend (`AUTH-10`).
///
/// <para>
/// <b>Vai ao banco em vez de devolver as claims do cookie</b>, e essa e' a escolha que importa: o
/// cookie e' valido ate expirar, entao um vinculo suspenso depois da emissao continuaria passando.
/// Reler o vinculo aqui e' o que faz `POST /users/{id}/deactivate` ter efeito na proxima carga da
/// tela, e nao daqui a oito horas. E' uma consulta por carga de aplicacao, nao por requisicao.
/// </para>
/// </summary>
public class GetCurrentUserHandler(IExecutionContext execution, IAuthenticatedUserReader reader)
  : IQueryHandler<GetCurrentUserQuery, Result<AuthenticatedUserDto>>
{
  private readonly IExecutionContext _execution = execution;
  private readonly IAuthenticatedUserReader _reader = reader;

  public async ValueTask<Result<AuthenticatedUserDto>> Handle(GetCurrentUserQuery query,
    CancellationToken cancellationToken)
  {
    if (_execution.UserId is not { } userId) return Result<AuthenticatedUserDto>.Unauthorized();

    var user = await _reader.ReadAsync(UserId.From(userId), cancellationToken);

    return user is null ? Result<AuthenticatedUserDto>.Unauthorized() : user;
  }
}
