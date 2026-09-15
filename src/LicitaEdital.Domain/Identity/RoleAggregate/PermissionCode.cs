namespace LicitaEdital.Domain.Identity.RoleAggregate;

/// <summary>
/// Catalogo fechado de permissoes. Os 5 primeiros codigos sao os que o frontend ja declara em
/// `core/permissions/permission.model.ts`; os demais vem da spec de participacao assistida (§16) e
/// ainda nao tem tela — estao aqui para que o catalogo nasca completo, nao para serem concedidos.
///
/// **O valor e' contrato**: ele viaja em `AuthUser.permissions` e o `permissionGuard` do Angular
/// compara string por string. Renomear um codigo quebra a tela sem erro de compilacao de nenhum
/// dos lados.
/// </summary>
public sealed class PermissionCode : SmartEnum<PermissionCode, string>
{
  // --- em uso pelo frontend hoje ---
  public static readonly PermissionCode UsersRead = new(nameof(UsersRead), "users.read");
  public static readonly PermissionCode UsersWrite = new(nameof(UsersWrite), "users.write");
  public static readonly PermissionCode CollectionsRead = new(nameof(CollectionsRead), "collections.read");
  public static readonly PermissionCode ParticipationOperationsRead = new(nameof(ParticipationOperationsRead), "participation-operations.read");
  public static readonly PermissionCode ParticipationPortalsManage = new(nameof(ParticipationPortalsManage), "participation-portals.manage");

  // --- previstas pela spec de participacao assistida (§16), sem tela ainda ---
  public static readonly PermissionCode ParticipationsRead = new(nameof(ParticipationsRead), "participations.read");
  public static readonly PermissionCode ParticipationsManage = new(nameof(ParticipationsManage), "participations.manage");
  public static readonly PermissionCode ParticipationsApprove = new(nameof(ParticipationsApprove), "participations.approve");
  public static readonly PermissionCode ParticipationsReportSubmission = new(nameof(ParticipationsReportSubmission), "participations.report-submission");
  public static readonly PermissionCode CompanyDocumentsRead = new(nameof(CompanyDocumentsRead), "company-documents.read");
  public static readonly PermissionCode CompanyDocumentsManage = new(nameof(CompanyDocumentsManage), "company-documents.manage");
  public static readonly PermissionCode CompanyDocumentsDownload = new(nameof(CompanyDocumentsDownload), "company-documents.download");
  public static readonly PermissionCode ParticipationPackagesDownload = new(nameof(ParticipationPackagesDownload), "participation-packages.download");
  public static readonly PermissionCode ParticipationOperationsRetry = new(nameof(ParticipationOperationsRetry), "participation-operations.retry");

  private PermissionCode(string name, string value) : base(name, value) { }
}
