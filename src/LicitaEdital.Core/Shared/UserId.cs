using Vogen;

namespace LicitaEdital.Core.Shared;

/// <summary>
/// Identidade do usuario autenticado. O registro em si (e-mail, hash de senha, tokens) pertence ao
/// ASP.NET Core Identity, em Infrastructure; o dominio so conhece o id.
/// </summary>
[ValueObject<Guid>]
public readonly partial struct UserId
{
  public static UserId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("UserId nao pode ser vazio.");
}
