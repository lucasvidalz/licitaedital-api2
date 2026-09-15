using LicitaEdital.BuildingBlocks.Domain.Identifiers;
using Vogen;

namespace LicitaEdital.Facade.Shared;

/// <summary>
/// Identidade da licitacao. Fica em Shared porque atravessa modulo: Catalog a possui, Engagement a
/// referencia em oportunidade salva e Participations (Fase 8) a usara como `TenderId`.
///
/// Referencia entre modulos e' **so o id** — sem propriedade de navegacao e sem FK entre schemas
/// (spec §4). O tipo compartilhado da seguranca de compilacao; ele nao cria acoplamento de dados.
/// </summary>
[ValueObject<Guid>]
public readonly partial struct OpportunityId : IGuidId<OpportunityId>
{
  private static Validation Validate(Guid value)
      => value != Guid.Empty ? Validation.Ok : Validation.Invalid("OpportunityId nao pode ser vazio.");
}
