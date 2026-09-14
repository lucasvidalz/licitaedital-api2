using LicitaEdital.Core.Shared;

namespace LicitaEdital.Core.Companies.CompanyProfileAggregate;

/// <summary>
/// Cadastro da empresa do cliente. **Singular por organizacao** — `GET /company-profile` nao recebe
/// id — e a ausencia dele e' o primeiro estado normal do ciclo de vida, nao uma falha: o endpoint
/// responde 404 e a tela abre formulario vazio (AD-032 do frontend). Nao logar esse 404 como erro.
///
/// Os 5 campos sao exatamente os de `CompanyProfileApiItem`. Nao acrescente coluna que a tela nao
/// pede (SEC-64) — o cadastro rico da empresa e' o assistente de registro, que hoje nem endpoint tem
/// (AD-041).
/// </summary>
public class CompanyProfile : EntityBase<CompanyProfile, CompanyProfileId>, IAggregateRoot
{
  private CompanyProfile(OrganizationId organizationId, CompanyName companyName, Cnpj cnpj,
    string city, StateCode state, string businessArea)
  {
    OrganizationId = organizationId;
    CompanyName = companyName;
    Cnpj = cnpj;
    City = city;
    State = state;
    BusinessArea = businessArea;
  }

  public OrganizationId OrganizationId { get; private set; }
  public CompanyName CompanyName { get; private set; }
  public Cnpj Cnpj { get; private set; }
  public string City { get; private set; }
  public StateCode State { get; private set; }
  public string BusinessArea { get; private set; }

  public DateTimeOffset CreatedAt { get; private set; }
  public DateTimeOffset UpdatedAt { get; private set; }

  public static CompanyProfile Create(OrganizationId organizationId, CompanyName companyName,
    Cnpj cnpj, string city, StateCode state, string businessArea, TimeProvider clock)
  {
    var now = clock.GetUtcNow();
    return new CompanyProfile(organizationId, companyName, cnpj, city, state, businessArea)
    {
      Id = CompanyProfileId.New(),
      CreatedAt = now,
      UpdatedAt = now
    };
  }

  /// <summary>`PUT /company-profile` e' upsert: quando o perfil ja existe, o corpo substitui tudo.</summary>
  public CompanyProfile Update(CompanyName companyName, Cnpj cnpj, string city, StateCode state,
    string businessArea, TimeProvider clock)
  {
    CompanyName = companyName;
    Cnpj = cnpj;
    City = city;
    State = state;
    BusinessArea = businessArea;
    UpdatedAt = clock.GetUtcNow();
    return this;
  }
}
