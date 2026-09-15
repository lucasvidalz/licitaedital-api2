using LicitaEdital.Domain.Collections.CollectionRunAggregate;

namespace LicitaEdital.Data.Collections.Config;

public class CollectionRunConfiguration : IEntityTypeConfiguration<CollectionRun>
{
  public void Configure(EntityTypeBuilder<CollectionRun> builder)
  {
    builder.ToTable("collection_runs");

    builder.Property(run => run.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(run => run.StartedAt).IsRequired();
    builder.Property(run => run.EndedAt);
    builder.Property(run => run.NewOpportunitiesCount).IsRequired();

    builder.Property(run => run.Result)
      .HasConversion(result => result.Value, value => SmartEnum<CollectionRunResult, string>.FromValue(value))
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    builder.Property(run => run.ErrorMessage).HasMaxLength(DataSchemaConstants.DefaultTextLength);

    // `GET /gfe/collections` pagina por inicio decrescente.
    builder.HasIndex(run => run.StartedAt).HasDatabaseName("ix_collection_runs_started_at");

    // Serve ao `lastSuccessfulRunAt` do envelope, que e' derivado — o maior `ended_at` entre as
    // execucoes com resultado `success`.
    builder.HasIndex(run => new { run.Result, run.EndedAt })
      .HasDatabaseName("ix_collection_runs_result_ended_at");
  }
}
