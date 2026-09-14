using Docker.DotNet;

namespace LicitaEdital.FunctionalTests;

public class DockerAvailabilityTests
{
  [Fact]
  public async Task Docker_ShouldBeRunning_ForFullFunctionalTestCoverage()
  {
    var cancellationToken = TestContext.Current.CancellationToken;
    try
    {
      // Ping the Docker daemon directly using the Docker client.
      // This has no side effects on container lifecycle or Testcontainers internals.
      using var client = new DockerClientBuilder().Build();
      await client.System.PingAsync(cancellationToken);
    }
    catch (Exception)
    {
      Assert.Fail(
        "Docker nao esta rodando ou esta mal configurado. " +
        "Os testes funcionais sobem PostgreSQL em container e **nao** tem fallback: sem Docker eles falham, " +
        "porque um banco em memoria nao exercita text[], xmin, schema por modulo nem indice unico. " +
        "Inicie o Docker (https://www.docker.com/products/docker-desktop/) e rode os testes de novo.");
    }
  }
}
