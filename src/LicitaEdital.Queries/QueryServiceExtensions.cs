using Ardalis.GuardClauses;
using LicitaEdital.Queries.Catalog;
using LicitaEdital.Queries.Collections;
using LicitaEdital.Queries.Companies;
using LicitaEdital.Queries.Engagement;
using LicitaEdital.Queries.Offerings;
using LicitaEdital.Queries.Identity;
using LicitaEdital.Queries.Contracts.Catalog;
using LicitaEdital.Queries.Contracts.Collections;
using LicitaEdital.Queries.Contracts.Companies;
using LicitaEdital.Queries.Contracts.Engagement;
using LicitaEdital.Queries.Contracts.Offerings;
using LicitaEdital.Queries.Contracts.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LicitaEdital.Queries;

public static class QueryServiceExtensions
{
  /// <summary>
  /// Connection string do lado de leitura. Quando ausente, cai na de escrita — que e' o caso hoje,
  /// com uma base so. O nome separado existe para que apontar uma replica de leitura amanha seja
  /// uma linha de configuracao, e nao uma refatoracao.
  /// </summary>
  public const string ReadConnectionStringName = "licitaedital-read";

  public static IServiceCollection AddQueryServices(this IServiceCollection services,
    IConfiguration config)
  {
    var connectionString = config.GetConnectionString(ReadConnectionStringName)
                           ?? config.GetConnectionString("licitaedital");
    Guard.Against.NullOrWhiteSpace(connectionString, nameof(connectionString));

    // Contexto de leitura por modulo. **Sem os interceptors** de auditoria, soft delete, tenant e
    // eventos: eles so fazem sentido quando ha gravacao, e este contexto recusa gravar.
    // Sem `MigrationsHistoryTable`: este contexto nunca migra. Quem cria e evolui o schema e' o
    // lado de escrita, e ter os dois apontando para a mesma tabela de historico so convidaria
    // alguem a rodar `dotnet ef` contra o contexto errado.
    services.AddDbContext<CatalogReadContext>(options => options.UseNpgsql(connectionString));

    services.AddDbContext<IdentityReadContext>(options => options.UseNpgsql(connectionString));
    services.AddDbContext<CompaniesReadContext>(options => options.UseNpgsql(connectionString));
    services.AddDbContext<OfferingsReadContext>(options => options.UseNpgsql(connectionString));
    services.AddDbContext<EngagementReadContext>(options => options.UseNpgsql(connectionString));
    services.AddDbContext<CollectionsReadContext>(options => options.UseNpgsql(connectionString));

    services.AddScoped<IListOpportunitiesQueryService, ListOpportunitiesQueryService>();
    services.AddScoped<IOpportunityDetailsQueryService, OpportunityDetailsQueryService>();
    services.AddScoped<IAuthenticatedUserReader, AuthenticatedUserReader>();
    services.AddScoped<IUsersQueryService, UsersQueryService>();
    services.AddScoped<ICompanyProfileQueryService, CompanyProfileQueryService>();
    services.AddScoped<IOfferingsQueryService, OfferingsQueryService>();
    services.AddScoped<ISavedOpportunitiesQueryService, SavedOpportunitiesQueryService>();
    services.AddScoped<IAlertPreferencesQueryService, AlertPreferencesQueryService>();
    services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
    services.AddScoped<ICollectionRunsQueryService, CollectionRunsQueryService>();
    services.AddScoped<ICoverageSettingsQueryService, CoverageSettingsQueryService>();

    return services;
  }
}
