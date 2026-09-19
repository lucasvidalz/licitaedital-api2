using Ardalis.SmartEnum;
using FluentValidation;
using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Api.Gfe;

public class CoverageSettingsRequest
{
  /// <summary>UFs cobertas. **Vazio significa todas**, nao nenhuma — e' cobertura, nao restricao.</summary>
  public IReadOnlyList<string> AttendedStates { get; set; } = [];

  /// <summary>Chave simbolica. <c>""</c> e' valida: sem faixa.</summary>
  public string ValueRange { get; set; } = string.Empty;
}

public class CoverageSettingsValidator : Validator<CoverageSettingsRequest>
{
  public CoverageSettingsValidator()
  {
    RuleFor(request => request.AttendedStates)
      .Must(states => states.All(StateCode.IsValid))
      .WithMessage("Há UF inválida na cobertura.");

    RuleFor(request => request.ValueRange)
      .Must(value => SmartEnum<ValueRange, string>.TryFromValue(value ?? string.Empty, out _))
      .WithMessage("Faixa de valor inválida.");
  }
}
