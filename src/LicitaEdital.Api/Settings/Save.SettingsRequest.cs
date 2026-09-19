using Ardalis.SmartEnum;
using FluentValidation;
using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.Domain.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Api.Settings;

public class SettingsRequest
{
  public AlertTypesPayload AlertTypes { get; set; } = new();
  public string Frequency { get; set; } = string.Empty;
  public SettingsFilterPayload Filter { get; set; } = new();
}

public class AlertTypesPayload
{
  public bool NewCompatibleOpportunity { get; set; }
  public bool ApproachingDeadline { get; set; }
  public bool DailySummary { get; set; }
}

public class SettingsFilterPayload
{
  public IReadOnlyList<string> States { get; set; } = [];
  public IReadOnlyList<string> Modalities { get; set; } = [];

  /// <summary>Chave simbolica. <c>""</c> e' valor valido: significa "sem faixa".</summary>
  public string ValueRange { get; set; } = string.Empty;
}

/// <summary>
/// Os tres catalogos fechados sao validados contra o dominio, nao contra uma lista repetida aqui:
/// frequencia, UF e faixa de valor. Valor desconhecido em qualquer um deles viraria excecao no meio
/// do caso de uso — 500 — em vez de 400 com o campo nomeado.
/// </summary>
public class SettingsValidator : Validator<SettingsRequest>
{
  public SettingsValidator()
  {
    RuleFor(request => request.Frequency)
      .NotEmpty()
      .Must(value => SmartEnum<AlertFrequency, string>.TryFromValue(value, out _))
      .WithMessage($"Frequência inválida. Use '{AlertFrequency.Immediate.Value}', " +
                   $"'{AlertFrequency.DailyDigest.Value}' ou '{AlertFrequency.WeeklyDigest.Value}'.");

    RuleFor(request => request.Filter.States)
      .Must(states => states.All(StateCode.IsValid))
      .WithName("filter.states")
      .WithMessage("Há UF inválida no filtro.");

    RuleFor(request => request.Filter.ValueRange)
      // `ValueRange` inclui a chave vazia (`Unset`), entao `TryFromValue("")` ja aceita "sem faixa" —
      // nao ha caso especial a tratar aqui.
      .Must(value => SmartEnum<ValueRange, string>.TryFromValue(value ?? string.Empty, out _))
      .WithName("filter.valueRange")
      .WithMessage("Faixa de valor inválida.");
  }
}
