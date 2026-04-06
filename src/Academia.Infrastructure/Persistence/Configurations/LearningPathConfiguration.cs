using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class LearningPathConfiguration : IEntityTypeConfiguration<LearningPath>
{
    public void Configure(EntityTypeBuilder<LearningPath> builder)
    {
        builder.ToTable("learning_paths");

        builder.HasKey(lp => lp.Id);

        builder.Property(lp => lp.Name).IsRequired().HasMaxLength(300);
        builder.Property(lp => lp.Description).HasMaxLength(2000);
        builder.Property(lp => lp.Image).HasMaxLength(500);
        builder.Property(lp => lp.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(lp => lp.AccessType).HasConversion<string>().HasMaxLength(50);
        builder.Property(lp => lp.Price).HasPrecision(10, 2);

        builder.HasIndex(lp => lp.Status);

        builder.HasMany(lp => lp.Courses)
            .WithOne(lpc => lpc.LearningPath)
            .HasForeignKey(lpc => lpc.LearningPathId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
