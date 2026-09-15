using Ardalis.ListStartupServices;
using LicitaEdital.BuildingBlocks.Persistence;
using LicitaEdital.BuildingBlocks.Web.Defaults;
using LicitaEdital.Infrastructure.Data.Catalog;
using LicitaEdital.Infrastructure.Data.Collections;
using LicitaEdital.Infrastructure.Data.Companies;
using LicitaEdital.Infrastructure.Data.Engagement;
using LicitaEdital.Infrastructure.Data.Identity;
using LicitaEdital.Infrastructure.Data.Offerings;
using Scalar.AspNetCore;

namespace LicitaEdital.Web.Configurations;

public static class MiddlewareConfig
{
  /// <summary>
  /// Contextos de modulo, na ordem em que sao migrados. Identity primeiro por convencao — nao ha
  /// dependencia entre schemas, mas uma ordem fixa torna o log de inicializacao comparavel entre
  /// execucoes.
  /// </summary>
  public static readonly IReadOnlyList<Type> ModuleContexts =
  [
    typeof(IdentityDbContext),
    typeof(CompaniesDbContext),
    typeof(CatalogDbContext),
    typeof(OfferingsDbContext),
    typeof(EngagementDbContext),
    typeof(CollectionsDbContext)
  ];

  public static async Task<IApplicationBuilder> UseAppMiddleware(this WebApplication app)
  {
    // Middlewares transversais, na ordem que importa: excecao, cabecalhos de seguranca, correlacao,
    // HTTPS, CORS, autenticacao e autorizacao. A ordem e' da lib, e nao configuravel — errar nela e
    // silencioso.
    app.UseBuildingBlocksWeb();
    app.UseRateLimiter();

    app.UseFastEndpoints();

    if (app.Environment.IsDevelopment())
    {
      app.UseShowAllServicesMiddleware(); // https://github.com/ardalis/AspNetCoreStartupServices

      app.UseSwaggerGen(options => options.Path = "/openapi/{documentName}.json",
        settings =>
        {
          settings.Path = "/swagger";
          settings.DocumentPath = "/openapi/{documentName}.json";
        });

      app.MapScalarApiReference(options =>
      {
        options.WithTitle("LicitaEdital API");
        options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
      });
    }

    var shouldMigrate = app.Environment.IsDevelopment() ||
                        app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");

    if (shouldMigrate)
    {
      // O laco dos seis contextos vive na lib: ele era identico aqui e na fixture de teste
      // funcional, e um contexto novo exigia lembrar de acrescentar nos dois lugares.
      await app.Services.MigrateAllAsync(ModuleContexts);

      // Semente de identidade logo depois: sem papel padrao, `POST /auth/register` nao tem o que
      // atribuir. Idempotente, entao roda a cada inicializacao sem duplicar.
      using var scope = app.Services.CreateScope();
      await IdentitySeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<IdentityDbContext>());
    }

    return app;
  }
}
