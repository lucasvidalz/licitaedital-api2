using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Core.Companies.CompanyProfileAggregate;

[ValueObject<Guid>]
public readonly partial struct CompanyProfileId : IGuidId<CompanyProfileId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("CompanyProfileId nao pode ser vazio.");
}
