using LicitaEdital.Core.Engagement.SubscriptionAggregate;

namespace LicitaEdital.Infrastructure.Data.Engagement.Config;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
  public void Configure(EntityTypeBuilder<Subscription> builder)
  {
    builder.ToTable("subscriptions");

    builder.Property(subscription => subscription.Id)
      .HasVogenConversion()
      .ValueGeneratedNever();

    builder.Property(subscription => subscription.OrganizationId).HasVogenConversion().IsRequired();

    builder.Property(subscription => subscription.PlanId)
      .HasConversion(plan => plan.Value, value => SmartEnum<PlanId, string>.FromValue(value))
      .HasMaxLength(DataSchemaConstants.DefaultCodeLength)
      .IsRequired();

    builder.Property(subscription => subscription.StartedAt).IsRequired();

    // Uma assinatura vigente por organizacao. Historico de troca de plano nao existe ainda —
    // quando existir, e' tabela propria, nao segunda linha aqui.
    builder.HasIndex(subscription => subscription.OrganizationId)
      .IsUnique()
      .HasDatabaseName("ux_subscriptions_organization")
      .ActiveOnly();

    builder.UseXminConcurrencyToken();
  }
}
