using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Core.Shared;

/// <summary>
/// Identidade do tenant. Vive em Shared, e nao no modulo Identity, porque **toda** tabela privada
/// do sistema a carrega (D-02) — Companies, Catalog, Offerings e Engagement referenciam a
/// organizacao sem poder depender do modulo que a possui.
/// </summary>
[ValueObject<Guid>]
public readonly partial struct OrganizationId : IGuidId<OrganizationId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("OrganizationId nao pode ser vazio.");
}
