namespace LicitaEdital.Data;

/// <summary>
/// Base das factories de design-time deste produto. A mecanica — ler a variavel de ambiente,
/// instanciar o contexto, apontar a tabela de historico — vem de
/// <see cref="LicitaEdital.BuildingBlocks.Persistence.ModuleDbContextFactory{TContext}"/>; o que
/// fica aqui e' a unica coisa que a lib nao pode saber: qual e' o banco.
/// </summary>
public abstract class NpgsqlModuleDbContextFactory<TContext> : ModuleDbContextFactory<TContext>
  where TContext : DbContext
{
  /// <summary>
  /// PostgreSQL local em container. Porta 5434, e nao 5432: nesta maquina a 5432 e' do PostgreSQL
  /// do sistema e a 5433 e' de outro projeto. CI e runner de migracao passam
  /// `ConnectionStrings__licitaedital` e nunca chegam neste default.
  /// </summary>
  protected override string FallbackConnectionString =>
    "Host=localhost;Port=5434;Database=licitaedital;Username=postgres";

  protected override void ConfigureProvider(DbContextOptionsBuilder builder, string connectionString,
    string schema)
    => builder.UseNpgsql(connectionString, npgsql =>
      npgsql.MigrationsHistoryTable(DataSchemaConstants.MigrationsHistoryTable, schema));
}
