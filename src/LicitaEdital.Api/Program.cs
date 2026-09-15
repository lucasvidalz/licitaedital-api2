using LicitaEdital.BuildingBlocks.Web.Defaults;
using LicitaEdital.BuildingBlocks.Web.Logging;
using LicitaEdital.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()             // OpenTelemetry, health checks e service discovery
       .AddBuildingBlocksLogging();      // Serilog no console, ao lado do OTel

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();
startupLogger.LogInformation("Starting web host");

// O que este app **escolhe**: quais modulos, quais assemblies e como se identifica. Fica visivel
// aqui de proposito — o que a lib absorveu foi o mecanismo, nao a decisao.
builder.Services.AddBuildingBlocksWeb(new BuildingBlocksWebOptions
{
  IsDevelopment = builder.Environment.IsDevelopment(),
  CorsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? []
});

builder.Services.AddRateLimitPolicies();
builder.Services.AddServiceConfigs(startupLogger, builder);

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.DocumentSettings = s =>
                  {
                    s.Title = "LicitaEdital API";
                    s.Version = "v1";
                    s.Description = "HTTP endpoints da API do LicitaEdital.";
                  };
                  o.ShortSchemaNames = true;
                });

var app = builder.Build();

await app.UseAppMiddleware();

app.MapDefaultEndpoints(); // health checks e metricas do Aspire

app.Run();

// Torna a classe Program publica para os testes funcionais referenciarem o assembly correto.
public partial class Program { }
