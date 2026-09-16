namespace LicitaEdital.Domain.Companies.CompanyProfileAggregate.Specifications;

/// <summary>
/// O perfil de um CNPJ, em **qualquer** organizacao.
///
/// <para>
/// E' a unica consulta do sistema que atravessa o tenant de proposito, e por um motivo: o CNPJ e'
/// unico na plataforma inteira (`ux_company_profiles_cnpj`), entao a colisao precisa ser detectada
/// antes do banco recusar. Ela devolve a linha para quem chama **so' comparar a organizacao** —
/// nenhum campo dela pode chegar a uma resposta HTTP, ou o cadastro viraria um verificador de quais
/// CNPJs ja estao na base.
/// </para>
/// </summary>
public sealed class CompanyProfileByCnpjSpec : SingleResultSpecification<CompanyProfile>
{
  public CompanyProfileByCnpjSpec(Cnpj cnpj)
  {
    Query.Where(profile => profile.Cnpj == cnpj);
  }
}
