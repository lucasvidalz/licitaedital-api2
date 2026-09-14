using Ardalis.ListStartupServices;
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
  public static async Task<IApplicationBuilder> UseAppMiddleware(this WebApplication app)
  {
    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
    }
    else
    {
      app.UseDefaultExceptionHandler(); // from FastEndpoints
      app.UseHsts();
    }

    app.UseFastEndpoints();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwaggerGen(options =>
      {
        options.Path = "/openapi/{documentName}.json";
      },
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

    app.UseHttpsRedirection(); // Note this will drop Authorization headers

    var shouldMigrate = app.Environment.IsDevelopment() ||
                        app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");

    if (shouldMigrate)
    {
      await MigrateDatabaseAsync(app);
    }

    return app;
  }

  /// <summary>
  /// Migra os seis contextos de modulo, em ordem fixa. Cada um tem sua propria tabela de historico
  /// no seu schema (D-01), entao as migracoes sao independentes — o que falha aqui e' o modulo, nao
  /// a base inteira.
  ///
  /// Nao ha caminho `EnsureCreated`: com PostgreSQL em todo ambiente, inclusive local via Aspire,
  /// criar o schema por atalho em desenvolvimento produziria uma base que **nao** corresponde as
  /// migracoes aplicadas em producao — o erro aparece so no deploy.
  /// </summary>
  private static async Task MigrateDatabaseAsync(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
      logger.LogInformation("Applying database migrations...");

      await services.GetRequiredService<IdentityDbContext>().Database.MigrateAsync();
      await services.GetRequiredService<CompaniesDbContext>().Database.MigrateAsync();
      await services.GetRequiredService<CatalogDbContext>().Database.MigrateAsync();
      await services.GetRequiredService<OfferingsDbContext>().Database.MigrateAsync();
      await services.GetRequiredService<EngagementDbContext>().Database.MigrateAsync();
      await services.GetRequiredService<CollectionsDbContext>().Database.MigrateAsync();

      logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred migrating the DB. {exceptionMessage}", ex.Message);
      throw; // Re-throw to make startup fail if migrations fail
    }
  }
}
