using Ardalis.SmartEnum;
using LicitaEdital.BuildingBlocks.Brasil;
using LicitaEdital.Domain.Collections.CoverageSettingsAggregate;
using LicitaEdital.Facade.Shared;
using LicitaEdital.Queries.Contracts.Collections;

namespace LicitaEdital.Application.Collections.Coverage.Save;

/// <summary>
/// Grava a cobertura do radar. Upsert idempotente sobre um **singleton**: ha no maximo uma linha, e
/// o id existe porque o EF precisa de chave, nao porque haja mais de uma cobertura.
/// </summary>
public class SaveCoverageSettingsHandler(
  IRepository<CoverageSettings> coverage,
  ICoverageSettingsQueryService reader)
  : ICommandHandler<SaveCoverageSettingsCommand, Result<CoverageSettingsDto>>
{
  private readonly IRepository<CoverageSettings> _coverage = coverage;
  private readonly ICoverageSettingsQueryService _reader = reader;

  public async ValueTask<Result<CoverageSettingsDto>> Handle(SaveCoverageSettingsCommand command,
    CancellationToken cancellationToken)
  {
    var states = command.AttendedStates.Select(StateCode.Parse);
    var valueRange = SmartEnum<ValueRange, string>.FromValue(command.ValueRange);

    var existing = (await _coverage.ListAsync(cancellationToken)).FirstOrDefault();

    if (existing is null)
    {
      existing = CoverageSettings.CreateDefault();
      existing.Update(states, valueRange);
      await _coverage.AddAsync(existing, cancellationToken);
    }
    else
    {
      existing.Update(states, valueRange);
      await _coverage.UpdateAsync(existing, cancellationToken);
    }

    return await _reader.GetAsync(cancellationToken);
  }
}
