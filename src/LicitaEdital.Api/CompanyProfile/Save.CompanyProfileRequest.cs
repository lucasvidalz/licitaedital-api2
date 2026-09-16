using FluentValidation;
using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.Data;
using LicitaEdital.Domain.Companies.CompanyProfileAggregate;

namespace LicitaEdital.Api.CompanyProfile;

public class CompanyProfileRequest
{
  public string CompanyName { get; set; } = string.Empty;
  public string Cnpj { get; set; } = string.Empty;
  public string City { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
  public string BusinessArea { get; set; } = string.Empty;
}

/// <summary>
/// Revalida no servidor o que o Angular ja valida como UX (SEC-30) — inclusive o CNPJ, que o
/// frontend confere em `validators/cnpj.validator.ts`.
///
/// <para>
/// <b>O CNPJ e validado pelo mesmo algoritmo do dominio</b> (<see cref="Cnpj.IsValid"/>, da lib), e
/// nao por uma regex de 14 digitos: o formato alfanumerico da IN RFB 2.229/2024 tem letra nas 12
/// primeiras posicoes, e uma regex numerica recusaria CNPJ legitimo a partir de julho de 2026.
/// </para>
///
/// <para>
/// A UF e validada contra a lista fechada de <see cref="StateCode"/>. Sem isso, "SS" entraria e so'
/// falharia no `StateCode.From` dentro do caso de uso — excecao, 500, em vez de 400 com o campo
/// nomeado.
/// </para>
/// </summary>
public class CompanyProfileValidator : Validator<CompanyProfileRequest>
{
  public CompanyProfileValidator()
  {
    RuleFor(request => request.CompanyName)
      .NotEmpty().MaximumLength(CompanyName.MaxLength);

    RuleFor(request => request.Cnpj)
      .NotEmpty()
      .Must(Cnpj.IsValid)
      .WithMessage("CNPJ inválido.");

    RuleFor(request => request.City)
      .NotEmpty().MaximumLength(DataSchemaConstants.DefaultNameLength);

    RuleFor(request => request.State)
      .NotEmpty()
      .Must(StateCode.IsValid)
      .WithMessage("UF inválida.");

    RuleFor(request => request.BusinessArea)
      .NotEmpty().MaximumLength(DataSchemaConstants.DefaultNameLength);
  }
}
