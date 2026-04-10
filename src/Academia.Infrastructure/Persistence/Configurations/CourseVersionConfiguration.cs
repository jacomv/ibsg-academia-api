using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class CourseVersionConfiguration : IEntityTypeConfiguration<CourseVersion>
{
    public void Configure(EntityTypeBuilder<CourseVersion> builder)
    {
        builder.ToTable("course_versions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.SnapshotJson).IsRequired();
        builder.Property(v => v.Reason).IsRequired().HasMaxLength(500);

        builder.HasIndex(v => new { v.CourseId, v.VersionNumber }).IsUnique();

        builder.HasOne(v => v.Course)
            .WithMany()
            .HasForeignKey(v => v.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Author)
            .WithMany()
            .HasForeignKey(v => v.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
