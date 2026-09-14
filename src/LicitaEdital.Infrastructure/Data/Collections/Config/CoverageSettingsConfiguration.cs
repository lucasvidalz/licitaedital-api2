using LicitaEdital.Core.Collections.CoverageSettingsAggregate;
using LicitaEdital.Core.Shared;
using LicitaEdital.Infrastructure.Data.Config;

namespace LicitaEdital.Infrastructure.Data.Collections.Config;

public class CoverageSettingsConfiguration : IEntityTypeConfiguration<CoverageSettings>
{
  public void Configure(EntityTypeBuilder<CoverageSettings> builder)
  {
    builder.ToTable("coverage_settings");

    builder.Property(settings => settings.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property<List<StateCode>>("_attendedStates")
      .HasColumnName("attended_states")
      .HasColumnType("text[]")
      .HasConversion(CollectionConverters.StateCodeList(), CollectionConverters.StateCodeListComparer())
      .UsePropertyAccessMode(PropertyAccessMode.Field)
      .IsRequired();

    builder.Property(settings => settings.ValueRange)
      .HasConversion(range => range.Value, value => SmartEnum<ValueRange, string>.FromValue(value))
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    builder.Property(settings => settings.UpdatedAt).IsRequired();

    builder.UseXminAsConcurrencyToken();
  }
}
