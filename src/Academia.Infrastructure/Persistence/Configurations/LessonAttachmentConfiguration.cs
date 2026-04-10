using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class LessonAttachmentConfiguration : IEntityTypeConfiguration<LessonAttachment>
{
    public void Configure(EntityTypeBuilder<LessonAttachment> builder)
    {
        builder.ToTable("lesson_attachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName).IsRequired().HasMaxLength(500);
        builder.Property(a => a.FileUrl).IsRequired().HasMaxLength(1000);
        builder.Property(a => a.FileType).IsRequired().HasMaxLength(100);

        builder.HasIndex(a => a.LessonId);

        builder.HasOne(a => a.Lesson)
            .WithMany()
            .HasForeignKey(a => a.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
