using System.Linq.Expressions;
using Ardalis.SmartEnum;
using FluentValidation;
using LicitaEdital.Application.Offerings;
using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.Data;
using LicitaEdital.Domain.Offerings.OfferingAggregate;

namespace LicitaEdital.Api.Offerings;

/// <summary>
/// O corpo de uma oferta. **O mesmo em criacao e atualizacao** — `offerings-api.service.ts` manda
/// `CreateOfferingRequest` nos dois —, porque `PUT` substitui a oferta inteira.
/// </summary>
public class OfferingRequest
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public IReadOnlyList<string> PositiveKeywords { get; set; } = [];
  public IReadOnlyList<string> NegativeKeywords { get; set; } = [];
  public IReadOnlyList<string> Synonyms { get; set; } = [];
  public string SupplyType { get; set; } = string.Empty;
  public IReadOnlyList<string> CatalogCodes { get; set; } = [];
  public long? MinValueCents { get; set; }
  public long? MaxValueCents { get; set; }
  public IReadOnlyList<string> ServedRegions { get; set; } = [];

  public OfferingPayload ToPayload() => new(
    Name, Description, PositiveKeywords, NegativeKeywords, Synonyms, SupplyType, CatalogCodes,
    MinValueCents, MaxValueCents, ServedRegions);
}

/// <summary>`PUT /offerings/{id}`: o mesmo corpo, mais o id vindo da rota.</summary>
public class UpdateOfferingRequest : OfferingRequest
{
  public Guid Id { get; set; }
}

/// <summary>
/// As regras da oferta, declaradas uma vez e aplicadas aos dois validadores.
///
/// <para>
/// Generico sobre <c>T : OfferingRequest</c> porque <c>Include</c> do FluentValidation exige um
/// <c>IValidator&lt;T&gt;</c> do **mesmo** T — um validador da classe base nao serve para a derivada,
/// e a alternativa seria repetir as regras e deixar criacao e atualizacao divergirem em silencio.
/// </para>
/// </summary>
public static class OfferingRules
{
  /// <summary>
  /// Teto de itens por lista. Nao e' regra de negocio, e' contencao: os quatro conjuntos viram
  /// `text[]` e alimentam o motor de compatibilidade, e um corpo com 50 mil termos e' um jeito
  /// barato de derrubar o calculo de score de toda a organizacao.
  /// </summary>
  public const int MaxTermsPerList = 200;

  public const int MaxTermLength = 120;

  public static void Apply<T>(AbstractValidator<T> validator) where T : OfferingRequest
  {
    validator.RuleFor(request => request.Name)
      .NotEmpty().MaximumLength(OfferingName.MaxLength);

    validator.RuleFor(request => request.Description)
      .NotEmpty().MaximumLength(DataSchemaConstants.DefaultTextLength);

    validator.RuleFor(request => request.SupplyType)
      .NotEmpty()
      .Must(value => SmartEnum<SupplyType, string>.TryFromValue(value, out _))
      .WithMessage($"Tipo de fornecimento inválido. Use '{SupplyType.Product.Value}', " +
                   $"'{SupplyType.Service.Value}' ou '{SupplyType.Both.Value}'.");

    Terms(validator, request => request.PositiveKeywords, nameof(OfferingRequest.PositiveKeywords));
    Terms(validator, request => request.NegativeKeywords, nameof(OfferingRequest.NegativeKeywords));
    Terms(validator, request => request.Synonyms, nameof(OfferingRequest.Synonyms));
    Terms(validator, request => request.CatalogCodes, nameof(OfferingRequest.CatalogCodes));

    validator.RuleFor(request => request.ServedRegions)
      .Must(regions => regions.All(StateCode.IsValid))
      .WithMessage("Há UF inválida na lista de regiões atendidas.");

    validator.RuleFor(request => request.MinValueCents)
      .GreaterThanOrEqualTo(0).When(request => request.MinValueCents is not null);

    validator.RuleFor(request => request.MaxValueCents)
      .GreaterThanOrEqualTo(0).When(request => request.MaxValueCents is not null);

    // Faixa invertida: o frontend ja barra em `offering-value-range.validator.ts`, e o servidor
    // repete porque uma faixa `min > max` nunca casaria com licitacao nenhuma — a oferta ficaria
    // gravada e silenciosamente inutil.
    validator.RuleFor(request => request)
      .Must(request => request.MinValueCents is null
                    || request.MaxValueCents is null
                    || request.MinValueCents <= request.MaxValueCents)
      .WithName(nameof(OfferingRequest.MaxValueCents))
      .WithMessage("O valor máximo não pode ser menor que o mínimo.");
  }

  private static void Terms<T>(AbstractValidator<T> validator,
    Expression<Func<T, IReadOnlyList<string>>> selector, string field) where T : OfferingRequest
  {
    validator.RuleFor(selector)
      .Must(list => list.Count <= MaxTermsPerList)
      .WithName(field)
      .WithMessage($"Máximo de {MaxTermsPerList} itens.");

    validator.RuleFor(selector)
      .Must(list => list.All(term => term.Length <= MaxTermLength))
      .WithName(field)
      .WithMessage($"Cada item deve ter até {MaxTermLength} caracteres.");
  }
}

public class OfferingValidator : Validator<OfferingRequest>
{
  public OfferingValidator() => OfferingRules.Apply(this);
}

public class UpdateOfferingValidator : Validator<UpdateOfferingRequest>
{
  public UpdateOfferingValidator() => OfferingRules.Apply(this);
}
