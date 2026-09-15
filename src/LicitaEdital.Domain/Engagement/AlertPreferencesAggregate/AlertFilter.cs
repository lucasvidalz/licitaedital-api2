using LicitaEdital.Facade.Shared;

namespace LicitaEdital.Domain.Engagement.AlertPreferencesAggregate;

/// <summary>
/// Recorte padrao do feed e dos alertas. E' **projecao de leitura**, nao tipo mapeado: os tres
/// campos ficam achatados em <see cref="AlertPreferences"/> como colunas proprias, porque um owned
/// type com duas colecoes dentro pediria json e nenhuma consulta do produto precisa entrar no
/// filtro — ele e' lido e gravado inteiro.
///
/// <see cref="ValueRange"/> guarda **a chave simbolica**, nao os centavos: a conversao e' do cliente
/// (`build-opportunities-query.helper.ts:11-17`), e persistir o intervalo congelaria a tabela de
/// faixas no dado historico.
///
/// <see cref="Modalities"/> guarda o **codigo** da modalidade, nunca o rotulo — AD-030 do frontend
/// nasceu de comparar rotulo contra codigo.
/// </summary>
public record AlertFilter(
  IReadOnlyCollection<StateCode> States,
  IReadOnlyCollection<string> Modalities,
  ValueRange ValueRange)
{
  public static AlertFilter Empty => new([], [], ValueRange.Unset);
}
