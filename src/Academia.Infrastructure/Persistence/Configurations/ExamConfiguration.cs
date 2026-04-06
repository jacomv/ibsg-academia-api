using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("exams");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.PassingScore).HasPrecision(5, 2);
        builder.Property(e => e.MaxAttempts).HasDefaultValue(0);

        builder.HasIndex(e => e.CourseId);
        builder.HasIndex(e => e.ChapterId);

        builder.HasOne(e => e.Course)
            .WithMany()
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Chapter)
            .WithOne(ch => ch.Exam)
            .HasForeignKey<Exam>(e => e.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Questions)
            .WithOne(q => q.Exam)
            .HasForeignKey(q => q.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
