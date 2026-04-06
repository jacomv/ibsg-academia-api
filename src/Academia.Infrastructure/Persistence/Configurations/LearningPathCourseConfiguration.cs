using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class LearningPathCourseConfiguration : IEntityTypeConfiguration<LearningPathCourse>
{
    public void Configure(EntityTypeBuilder<LearningPathCourse> builder)
    {
        builder.ToTable("learning_path_courses");

        builder.HasKey(lpc => lpc.Id);

        builder.HasIndex(lpc => new { lpc.LearningPathId, lpc.CourseId }).IsUnique();
        builder.HasIndex(lpc => new { lpc.LearningPathId, lpc.Order });

        builder.HasOne(lpc => lpc.Course)
            .WithMany(c => c.LearningPaths)
            .HasForeignKey(lpc => lpc.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
