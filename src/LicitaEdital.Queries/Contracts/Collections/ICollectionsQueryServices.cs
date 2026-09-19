namespace LicitaEdital.Queries.Contracts.Collections;

/// <summary>
/// Historico de execucoes do coletor.
///
/// <para>
/// <b>Lista so' execucao concluida.</b> O contrato declara `endedAt: string` nao anulavel, e uma
/// execucao em andamento nao tem fim — inclui-la mandaria nulo num campo que a tela imprime direto.
/// A consequencia aceita: uma coleta em curso so' aparece quando termina. Se o painel precisar
/// mostrar "rodando agora", o contrato ganha um campo proprio para isso, e nao um `endedAt` nulo.
/// </para>
/// </summary>
public interface ICollectionRunsQueryService
{
  Task<CollectionRunsDto> ListAsync(PageRequest page, CancellationToken cancellationToken = default);
}

/// <summary>
/// Cobertura do radar. **Nunca devolve nulo**: sem linha gravada, o padrao do dominio — a tela de
/// cobertura nao tem estado "ainda nao configurado".
/// </summary>
public interface ICoverageSettingsQueryService
{
  Task<CoverageSettingsDto> GetAsync(CancellationToken cancellationToken = default);
}
