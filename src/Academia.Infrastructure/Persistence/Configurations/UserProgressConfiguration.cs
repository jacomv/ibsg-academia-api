using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class UserProgressConfiguration : IEntityTypeConfiguration<UserProgress>
{
    public void Configure(EntityTypeBuilder<UserProgress> builder)
    {
        builder.ToTable("user_progress");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(up => up.ProgressPercentage).HasPrecision(5, 2).HasDefaultValue(0m);

        // Each user can only have one progress record per lesson
        builder.HasIndex(up => new { up.UserId, up.LessonId }).IsUnique();
        builder.HasIndex(up => new { up.UserId, up.Status });

        builder.HasOne(up => up.Lesson)
            .WithMany()
            .HasForeignKey(up => up.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
