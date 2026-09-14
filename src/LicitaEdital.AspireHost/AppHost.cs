using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
var sqlServer = builder.AddSqlServer("sqlserver")
  .WithLifetime(ContainerLifetime.Persistent);

// Add the database
var licitaEditalDb = sqlServer.AddDatabase("licitaedital");

// Papercut SMTP container for email testing
var papercut = builder.AddContainer("papercut", "jijiechen/papercut", "latest")
  .WithEndpoint("smtp", e =>
  {
    e.TargetPort = 25;   // container port
    e.Port = 25;         // host port
    e.Protocol = ProtocolType.Tcp;
    e.UriScheme = "smtp";
  })
  .WithEndpoint("ui", e =>
  {
    e.TargetPort = 37408;
    e.Port = 37408;
    e.UriScheme = "http";
  });

// Add the web project with the database connection
builder.AddProject<Projects.LicitaEdital_Web>("web")
  .WithReference(licitaEditalDb)
  .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
  .WithEnvironment("Papercut__Smtp__Url", papercut.GetEndpoint("smtp"))
  .WaitFor(licitaEditalDb)
  .WaitFor(papercut);

builder
  .Build()
  .Run();
