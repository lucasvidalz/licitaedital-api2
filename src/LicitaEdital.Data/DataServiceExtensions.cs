using LicitaEdital.Domain.Catalog.CompatibilityAggregate;
using LicitaEdital.Domain.Catalog.OpportunityAggregate;
using LicitaEdital.Domain.Collections.CollectionRunAggregate;
using LicitaEdital.Domain.Collections.CoverageSettingsAggregate;
using LicitaEdital.Domain.Companies.CompanyProfileAggregate;
using LicitaEdital.Domain.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Domain.Engagement.SavedOpportunityAggregate;
using LicitaEdital.Domain.Engagement.SubscriptionAggregate;
using LicitaEdital.Domain.Identity.MembershipAggregate;
using LicitaEdital.Domain.Identity.OrganizationAggregate;
using LicitaEdital.Domain.Identity.RoleAggregate;
using LicitaEdital.Domain.Offerings.OfferingAggregate;
using LicitaEdital.Data.Catalog;
using LicitaEdital.Data.Collections;
using LicitaEdital.Data.Companies;
using LicitaEdital.Data.Engagement;
using LicitaEdital.Data.Identity;
using LicitaEdital.Data.Offerings;

namespace LicitaEdital.Data;

public static class DataServiceExtensions
{
  /// <summary>Nome da connection string. Fornecida pelo Aspire via <c>.WithReference(licitaEditalDb)</c>.</summary>
  public const string ConnectionStringName = "licitaedital";

  public static IServiceCollection AddDataServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    var connectionString = config.GetConnectionString(ConnectionStringName);
    Guard.Against.NullOrWhiteSpace(connectionString, nameof(connectionString));

    // Auditoria, soft delete, guarda de tenant e despacho de eventos vem da lib.
    services.AddBuildingBlocksPersistence();

    // Seis contextos, uma base, um schema cada (D-01). Cada um leva sua **propria** tabela de
    // historico de migracao, no seu schema: com a tabela default compartilhada, aplicar a migracao
    // de um modulo faria o EF considerar as dos outros como pendentes e tentar reaplica-las.
    services.AddModuleDbContext<IdentityDbContext>(connectionString, DataSchemaConstants.IdentitySchema);
    services.AddModuleDbContext<CompaniesDbContext>(connectionString, DataSchemaConstants.CompaniesSchema);
    services.AddModuleDbContext<CatalogDbContext>(connectionString, DataSchemaConstants.CatalogSchema);
    services.AddModuleDbContext<OfferingsDbContext>(connectionString, DataSchemaConstants.OfferingsSchema);
    services.AddModuleDbContext<EngagementDbContext>(connectionString, DataSchemaConstants.EngagementSchema);
    services.AddModuleDbContext<CollectionsDbContext>(connectionString, DataSchemaConstants.CollectionsSchema);

    // Cada agregado e' registrado **fechado**, apontando para o contexto do seu modulo. Nao da para
    // registrar `IRepository<>` aberto como num template de contexto unico: o DI nao teria como
    // escolher entre os seis, e a escolha errada leria a tabela de outro schema.
    services
      .AddAggregate<IdentityDbContext, Organization>()
      .AddAggregate<IdentityDbContext, Membership>()
      .AddAggregate<IdentityDbContext, Role>()
      .AddAggregate<CompaniesDbContext, CompanyProfile>()
      .AddAggregate<CatalogDbContext, Opportunity>()
      .AddAggregate<CatalogDbContext, OpportunityCompatibility>()
      .AddAggregate<OfferingsDbContext, Offering>()
      .AddAggregate<EngagementDbContext, SavedOpportunity>()
      .AddAggregate<EngagementDbContext, AlertPreferences>()
      .AddAggregate<EngagementDbContext, Subscription>()
      .AddAggregate<CollectionsDbContext, CollectionRun>()
      .AddAggregate<CollectionsDbContext, CoverageSettings>();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }

  private static IServiceCollection AddModuleDbContext<TContext>(this IServiceCollection services,
    string connectionString, string schema) where TContext : DbContext
    => services.AddDbContext<TContext>((provider, options) =>
    {
      options.UseNpgsql(connectionString, npgsql =>
        npgsql.MigrationsHistoryTable(DataSchemaConstants.MigrationsHistoryTable, schema));

      options.AddBuildingBlocksInterceptors(provider);
    });

  private static IServiceCollection AddAggregate<TContext, TAggregate>(this IServiceCollection services)
    where TContext : DbContext
    where TAggregate : class, IAggregateRoot
    => services
      .AddScoped<IRepository<TAggregate>, EfRepository<TContext, TAggregate>>()
      .AddScoped<IReadRepository<TAggregate>, EfRepository<TContext, TAggregate>>();
}
