namespace LicitaEdital.Queries.Identity;

/// <summary>
/// Uma linha da tela de usuarios: vinculo + conta + organizacao, ja juntos.
///
/// <para>
/// <b>E' um read model sobre SQL escrito a mao</b> (ver
/// <c>IdentityReadContext.OnModelCreating</c>), e nao tres entidades unidas por LINQ. O motivo e'
/// concreto: <c>Membership.UserId</c> e <c>Membership.OrganizationId</c> sao value objects do Vogen
/// com conversor, e o EF nao traduz um <c>join</c> cuja chave passa por conversor — o compilador
/// infere <c>object</c> para a chave e a consulta morre em tempo de execucao, nao de compilacao. Um
/// <c>join</c> entre <c>identity.users</c> (<c>Guid</c> cru, do ASP.NET Identity) e
/// <c>identity.memberships</c> (<c>UserId</c>) e' exatamente esse caso.
/// </para>
///
/// <para>
/// <b>O preco, declarado:</b> os nomes de coluna ficam em texto. Renomear uma coluna nao quebra a
/// compilacao — quebra esta consulta na primeira chamada. Em troca, sai um unico <c>SELECT</c>, sem
/// conversor no caminho, e nenhuma coluna de credencial e' lida (a tabela de usuarios tem
/// <c>password_hash</c> e <c>security_stamp</c>, e o SQL abaixo nao os menciona).
/// </para>
///
/// <para>
/// <b>A exclusao logica esta no proprio SQL</b>, e nao no filtro global: entidade sem chave nao
/// recebe filtro de consulta. Tirar o <c>is_active</c> de la' faria vinculo removido reaparecer na
/// listagem.
/// </para>
/// </summary>
public sealed class UserDirectoryRow
{
  public Guid UserId { get; private set; }
  public Guid OrganizationId { get; private set; }
  public string Email { get; private set; } = string.Empty;
  public string DisplayName { get; private set; } = string.Empty;
  public string Status { get; private set; } = string.Empty;
  public DateTimeOffset CreatedAt { get; private set; }
  public DateTimeOffset? LastLoginAt { get; private set; }
  public string CompanyName { get; private set; } = string.Empty;
}
