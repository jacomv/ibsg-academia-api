using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class ReorderAuditConfiguration : IEntityTypeConfiguration<ReorderAudit>
{
    public void Configure(EntityTypeBuilder<ReorderAudit> builder)
    {
        builder.ToTable("reorder_audits");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(50);
        builder.Property(a => a.PreviousOrderJson).IsRequired();
        builder.Property(a => a.NewOrderJson).IsRequired();

        builder.HasIndex(a => a.ParentId);

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
