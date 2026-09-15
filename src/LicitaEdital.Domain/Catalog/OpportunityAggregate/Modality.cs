namespace LicitaEdital.Domain.Catalog.OpportunityAggregate;

/// <summary>
/// Modalidade da licitacao, como a fonte publica: codigo estavel mais rotulo legivel.
///
/// Os dois campos viajam juntos no contrato (`OpportunityModalityApi`) e o filtro do frontend usa
/// o **codigo** como chave (AD-030). Nao substituir por SmartEnum: a Lei 14.133 e as fontes
/// publicam modalidades que o sistema precisa exibir sem conhecer de antemao — um enum fechado
/// transformaria fonte nova em excecao.
/// </summary>
public record Modality(string Code, string Label)
{
  public const int MaxCodeLength = 40;
  public const int MaxLabelLength = 120;
}
