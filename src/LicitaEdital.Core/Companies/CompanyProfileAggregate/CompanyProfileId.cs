using Vogen;

namespace LicitaEdital.Core.Companies.CompanyProfileAggregate;

[ValueObject<Guid>]
public readonly partial struct CompanyProfileId
{
  public static CompanyProfileId New() => From(Guid.CreateVersion7());

  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("CompanyProfileId nao pode ser vazio.");
}
