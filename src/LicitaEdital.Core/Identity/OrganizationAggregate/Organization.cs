using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Identity.OrganizationAggregate;

/// <summary>
/// O tenant. Toda tabela privada do sistema referencia uma organizacao, e nenhuma consulta de dado
/// privado roda sem filtrar por ela de forma explicita (spec §16 — global query filter e camada
/// adicional, nunca a principal).
///
/// A area de gerenciamento (GFE) tambem e' representada por uma organizacao, marcada
/// <see cref="IsPlatform"/>: o time interno precisa de sessao, membership e permissao como qualquer
/// outro usuario, e tratar "staff" como ausencia de tenant abriria um caminho sem filtro.
/// </summary>
public class Organization : EntityBase<Organization, OrganizationId>, IAggregateRoot
{
  private Organization(OrganizationName name, bool isPlatform)
  {
    Name = name;
    IsPlatform = isPlatform;
  }

  public OrganizationName Name { get; private set; }

  /// <summary>Organizacao do time interno (area `manager`), nao de um cliente.</summary>
  public bool IsPlatform { get; private set; }

  public DateTimeOffset CreatedAt { get; private set; }
  public DateTimeOffset UpdatedAt { get; private set; }

  public static Organization ForClient(OrganizationName name, TimeProvider clock)
  {
    var now = clock.GetUtcNow();
    return new Organization(name, isPlatform: false) { Id = OrganizationId.New(), CreatedAt = now, UpdatedAt = now };
  }

  public static Organization ForPlatform(OrganizationName name, TimeProvider clock)
  {
    var now = clock.GetUtcNow();
    return new Organization(name, isPlatform: true) { Id = OrganizationId.New(), CreatedAt = now, UpdatedAt = now };
  }

  public Organization Rename(OrganizationName newName, TimeProvider clock)
  {
    if (Name == newName) return this;
    Name = newName;
    UpdatedAt = clock.GetUtcNow();
    return this;
  }
}
