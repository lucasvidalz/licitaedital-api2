using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Identity.OrganizationAggregate;

/// <summary>
/// O tenant. Toda tabela privada do sistema referencia uma organizacao, e nenhuma consulta de dado
/// privado roda sem filtrar por ela de forma explicita (spec §16 — global query filter e camada
/// adicional, nunca a principal).
///
/// A area de gerenciamento (GFE) tambem e' uma organizacao, marcada <see cref="IsPlatform"/>: o time
/// interno precisa de sessao, membership e permissao como qualquer outro usuario, e tratar "staff"
/// como ausencia de tenant abriria um caminho de consulta sem filtro.
///
/// Id, <c>CreatedAt</c>, <c>UpdatedAt</c> e <c>IsActive</c> vem de <c>AggregateRoot</c> — nao os
/// redeclare.
/// </summary>
public class Organization : AggregateRoot<OrganizationId>
{
  private Organization(OrganizationName name, bool isPlatform)
  {
    Name = name;
    IsPlatform = isPlatform;
  }

  public OrganizationName Name { get; private set; }

  /// <summary>Organizacao do time interno (area `manager`), nao de um cliente.</summary>
  public bool IsPlatform { get; private set; }

  public static Organization ForClient(OrganizationName name) => new(name, isPlatform: false);

  public static Organization ForPlatform(OrganizationName name) => new(name, isPlatform: true);

  public Organization Rename(OrganizationName newName)
  {
    if (Name == newName) return this;
    Name = newName;
    return this;
  }
}
