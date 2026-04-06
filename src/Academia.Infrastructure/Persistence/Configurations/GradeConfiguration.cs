using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.ToTable("grades");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.TotalScore).HasPrecision(5, 2);
        builder.Property(g => g.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(g => g.TeacherFeedback).HasMaxLength(3000);

        builder.HasIndex(g => new { g.UserId, g.ExamId, g.AttemptNumber }).IsUnique();
        builder.HasIndex(g => new { g.UserId, g.Status });
        builder.HasIndex(g => g.GradedByTeacherId);

        builder.HasOne(g => g.Exam)
            .WithMany()
            .HasForeignKey(g => g.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.GradedByTeacher)
            .WithMany()
            .HasForeignKey(g => g.GradedByTeacherId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
