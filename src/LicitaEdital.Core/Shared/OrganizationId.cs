using Vogen;

namespace LicitaEdital.Core.Shared;

/// <summary>
/// Identidade do tenant. Vive em Shared, e nao no modulo Identity, porque **toda** tabela privada
/// do sistema a carrega (D-02) — Companies, Catalog, Offerings e Engagement referenciam a
/// organizacao sem poder depender do modulo que a possui.
/// </summary>
[ValueObject<Guid>]
public readonly partial struct OrganizationId
{
  public static OrganizationId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("OrganizationId nao pode ser vazio.");
}
