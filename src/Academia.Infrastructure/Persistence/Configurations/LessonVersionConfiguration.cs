using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class LessonVersionConfiguration : IEntityTypeConfiguration<LessonVersion>
{
    public void Configure(EntityTypeBuilder<LessonVersion> builder)
    {
        builder.ToTable("lesson_versions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.SnapshotJson).IsRequired();
        builder.Property(v => v.Reason).IsRequired().HasMaxLength(500);

        builder.HasIndex(v => new { v.LessonId, v.VersionNumber }).IsUnique();

        builder.HasOne(v => v.Lesson)
            .WithMany()
            .HasForeignKey(v => v.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Author)
            .WithMany()
            .HasForeignKey(v => v.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
