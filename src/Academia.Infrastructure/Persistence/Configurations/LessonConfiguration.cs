using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("lessons");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title).IsRequired().HasMaxLength(300);
        builder.Property(l => l.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(l => l.VideoUrl).HasMaxLength(1000);
        builder.Property(l => l.AudioUrl).HasMaxLength(1000);
        builder.Property(l => l.PdfFile).HasMaxLength(500);
        builder.Property(l => l.RequiresCompletingPrevious).HasDefaultValue(false);

        builder.HasIndex(l => new { l.ChapterId, l.Order });
    }
}
