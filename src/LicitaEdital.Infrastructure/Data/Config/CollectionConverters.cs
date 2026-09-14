using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using LicitaEdital.Core.Shared;

namespace LicitaEdital.Infrastructure.Data.Config;

/// <summary>
/// Conversores para colecao de SmartEnum e de value object, que o Npgsql grava como <c>text[]</c>
/// nativo — nao como JSON e nao como tabela filha.
///
/// <c>text[]</c> porque o PostgreSQL filtra array com operador proprio (<c>&amp;&amp;</c>, <c>@&gt;</c>) e
/// indexa com GIN. Uma tabela filha para "UFs atendidas" custaria um join em toda leitura de um
/// dado que sempre e' lido inteiro; JSON perderia o operador de array.
///
/// O <c>ValueComparer</c> nao e' opcional: sem ele o EF compara a colecao por referencia, e mudar
/// um item da lista nao marca a entidade como alterada — a gravacao some sem erro.
/// </summary>
public static class CollectionConverters
{
  public static ValueConverter<List<TEnum>, string[]> SmartEnumList<TEnum>()
    where TEnum : SmartEnum<TEnum, string>
    => new(
      list => list.Select(item => item.Value).ToArray(),
      array => array.Select(SmartEnum<TEnum, string>.FromValue).ToList());

  public static ValueComparer<List<TEnum>> SmartEnumListComparer<TEnum>()
    where TEnum : SmartEnum<TEnum, string>
    => new(
      (left, right) => left!.Select(i => i.Value).SequenceEqual(right!.Select(i => i.Value)),
      list => list.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.Value.GetHashCode())),
      list => list.ToList());

  public static ValueConverter<List<StateCode>, string[]> StateCodeList()
    => new(
      list => list.Select(item => item.Value).ToArray(),
      array => array.Select(StateCode.From).ToList());

  public static ValueComparer<List<StateCode>> StateCodeListComparer()
    => new(
      (left, right) => left!.Select(i => i.Value).SequenceEqual(right!.Select(i => i.Value)),
      list => list.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.Value.GetHashCode())),
      list => list.ToList());
}
