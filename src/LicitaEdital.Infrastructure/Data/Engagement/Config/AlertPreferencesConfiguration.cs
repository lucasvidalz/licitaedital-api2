using LicitaEdital.Core.Engagement.AlertPreferencesAggregate;
using LicitaEdital.Core.Shared;

namespace LicitaEdital.Infrastructure.Data.Engagement.Config;

public class AlertPreferencesConfiguration : IEntityTypeConfiguration<AlertPreferences>
{
  public void Configure(EntityTypeBuilder<AlertPreferences> builder)
  {
    builder.ToTable("alert_preferences");

    builder.Property(preferences => preferences.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(preferences => preferences.OrganizationId).HasVogenConversion().IsRequired();

    // Tres bandeiras que so fazem sentido juntas, e que o contrato envia aninhadas em `alertTypes`.
    builder.OwnsOne(preferences => preferences.Types, types =>
    {
      types.Property(value => value.NewCompatibleOpportunity)
        .HasColumnName("alert_new_compatible_opportunity").IsRequired();
      types.Property(value => value.ApproachingDeadline)
        .HasColumnName("alert_approaching_deadline").IsRequired();
      types.Property(value => value.DailySummary)
        .HasColumnName("alert_daily_summary").IsRequired();
    });
    builder.Navigation(preferences => preferences.Types).IsRequired();

    builder.Property(preferences => preferences.Frequency)
      .HasConversion(frequency => frequency.Value, value => SmartEnum<AlertFrequency, string>.FromValue(value))
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    // O filtro fica achatado em tres colunas — ver a nota em `AlertFilter`. `AlertFilter` em si e'
    // projecao de leitura e nao entra no modelo.
    builder.Property<List<StateCode>>("_filterStates")
      .HasColumnName("filter_states")
      .HasColumnType("text[]")
      .HasConversion(BrasilValueConverters.StateCodeList(), BrasilValueConverters.StateCodeListComparer())
      .UsePropertyAccessMode(PropertyAccessMode.Field)
      .IsRequired();

    builder.PrimitiveCollection<List<string>>("_filterModalities")
      .HasColumnName("filter_modalities")
      .HasColumnType("text[]")
      .UsePropertyAccessMode(PropertyAccessMode.Field)
      .IsRequired();

    builder.Property(preferences => preferences.FilterValueRange)
      .HasConversion(range => range.Value, value => SmartEnum<ValueRange, string>.FromValue(value))
      .HasColumnName("filter_value_range")
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    builder.Ignore(preferences => preferences.Filter);

    builder.HasIndex(preferences => preferences.OrganizationId)
      .IsUnique()
      .HasDatabaseName("ux_alert_preferences_organization")
      .ActiveOnly();

    builder.UseXminConcurrencyToken();
  }
}
