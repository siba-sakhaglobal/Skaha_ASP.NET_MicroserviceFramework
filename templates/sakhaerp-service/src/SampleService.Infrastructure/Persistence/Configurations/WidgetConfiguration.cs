namespace SampleService.Infrastructure.Persistence.Configurations;

using SampleService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class WidgetConfiguration : IEntityTypeConfiguration<Widget>
{
    public void Configure(EntityTypeBuilder<Widget> builder)
    {
        builder.ToTable("widgets");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Code).HasMaxLength(50).IsRequired();
        builder.Property(w => w.Name).HasMaxLength(200).IsRequired();
        builder.Property(w => w.Quantity).HasColumnType("numeric(18,4)");
        builder.HasIndex(w => new { w.TenantId, w.Code }).IsUnique();
    }
}
