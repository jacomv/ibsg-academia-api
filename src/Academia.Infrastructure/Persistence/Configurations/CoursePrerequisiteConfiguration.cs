using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class CoursePrerequisiteConfiguration : IEntityTypeConfiguration<CoursePrerequisite>
{
    public void Configure(EntityTypeBuilder<CoursePrerequisite> builder)
    {
        builder.ToTable("course_prerequisites");

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => new { p.CourseId, p.PrerequisiteCourseId }).IsUnique();

        builder.HasOne(p => p.Course)
            .WithMany()
            .HasForeignKey(p => p.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.PrerequisiteCourse)
            .WithMany()
            .HasForeignKey(p => p.PrerequisiteCourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
