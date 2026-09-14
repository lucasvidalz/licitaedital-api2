using Microsoft.EntityFrameworkCore.Metadata;

namespace LicitaEdital.Infrastructure.Data.Conventions;

/// <summary>
/// Renomeia tabelas, colunas, chaves e indices para <c>snake_case</c>.
///
/// Nao e' preferencia estetica: o PostgreSQL dobra identificador nao-citado para minusculo, entao
/// <c>PascalCase</c> so sobrevive entre aspas duplas — e todo SQL escrito a mao, todo `psql` e todo
/// script de operacao passa a exigir aspas para ler a propria tabela.
///
/// Escrito a mao, e nao via `EFCore.NamingConventions`, para nao acrescentar dependencia por uma
/// regra de 40 linhas. Se o pacote entrar por outro motivo, este arquivo sai.
/// </summary>
public static class SnakeCaseNaming
{
  public static ModelBuilder UseSnakeCaseNames(this ModelBuilder modelBuilder)
  {
    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
      var table = entity.GetTableName();
      if (table is not null)
      {
        entity.SetTableName(ToSnakeCase(table));
      }

      // O nome default de uma coluna de owned type carrega o prefixo do dono (`Types_DailySummary`),
      // e esse prefixo so existe no nome de loja — nunca em `property.Name`. Ler pelo
      // StoreObjectIdentifier preserva o prefixo; ler pelo nome da propriedade o perderia, e duas
      // colunas de owned types diferentes colidiriam.
      var storeObject = StoreObjectIdentifier.Create(entity, StoreObjectType.Table);
      foreach (var property in entity.GetProperties())
      {
        var current = storeObject is null
          ? property.Name
          : property.GetColumnName(storeObject.Value) ?? property.Name;
        property.SetColumnName(ToSnakeCase(current));
      }

      foreach (var key in entity.GetKeys())
      {
        var name = key.GetName();
        if (name is not null) key.SetName(ToSnakeCase(name));
      }

      foreach (var foreignKey in entity.GetForeignKeys())
      {
        var name = foreignKey.GetConstraintName();
        if (name is not null) foreignKey.SetConstraintName(ToSnakeCase(name));
      }

      foreach (var index in entity.GetIndexes())
      {
        var name = index.GetDatabaseName();
        if (name is not null) index.SetDatabaseName(ToSnakeCase(name));
      }
    }

    return modelBuilder;
  }

  private static string ToSnakeCase(string name)
  {
    var builder = new System.Text.StringBuilder(name.Length + 8);

    for (var i = 0; i < name.Length; i++)
    {
      var current = name[i];

      if (current == '_')
      {
        if (builder.Length > 0 && builder[^1] != '_') builder.Append('_');
        continue;
      }

      if (char.IsUpper(current) && builder.Length > 0 && builder[^1] != '_')
      {
        var previous = name[i - 1];
        var nextIsLower = i + 1 < name.Length && char.IsLower(name[i + 1]);
        if (!char.IsUpper(previous) || nextIsLower) builder.Append('_');
      }

      builder.Append(char.ToLowerInvariant(current));
    }

    return builder.ToString();
  }
}
