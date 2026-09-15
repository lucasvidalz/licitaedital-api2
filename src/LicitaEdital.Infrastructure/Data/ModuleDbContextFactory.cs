using Microsoft.EntityFrameworkCore.Design;

namespace LicitaEdital.Infrastructure.Data;

/// <summary>
/// Cria um contexto de modulo em **tempo de design**, para `dotnet ef migrations` e
/// `dotnet ef database update`.
///
/// <para>
/// Sem isto, o EF tenta subir o host do `Web` para achar o contexto no DI. Isso funciona num
/// projeto de contexto unico e falha aqui por dois motivos: o host inteiro precisa iniciar so' para
/// gerar uma migracao — puxando FastEndpoints, autenticacao e providers junto —, e qualquer falha
/// nesse caminho vira um erro de migracao que nao tem nada a ver com migracao. A mensagem
/// "FastEndpoints was unable to find any endpoint declarations" travando um `migrations add` e' o
/// exemplo exato.
/// </para>
///
/// A connection string vem de `ConnectionStrings__licitaedital` quando definida — e' como o CI e o
/// runner de migracao a passam —, e cai no PostgreSQL local de desenvolvimento quando nao.
/// </summary>
public abstract class ModuleDbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext>
  where TContext : DbContext
{
  private const string LocalDevelopmentConnectionString =
    "Host=localhost;Port=5434;Database=licitaedital;Username=postgres";

  /// <summary>Schema do modulo. Define onde a tabela de historico de migracao e' criada.</summary>
  protected abstract string Schema { get; }

  public TContext CreateDbContext(string[] args)
  {
    var connectionString =
      Environment.GetEnvironmentVariable("ConnectionStrings__licitaedital")
      ?? LocalDevelopmentConnectionString;

    var options = new DbContextOptionsBuilder<TContext>()
      .UseNpgsql(connectionString, npgsql =>
        npgsql.MigrationsHistoryTable(DataSchemaConstants.MigrationsHistoryTable, Schema))
      .Options;

    // Sem interceptor: em tempo de design nao ha sessao, nao ha usuario e nao ha o que auditar.
    return (TContext)Activator.CreateInstance(typeof(TContext), options)!;
  }
}
