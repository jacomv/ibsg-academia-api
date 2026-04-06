using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class ExamAnswerConfiguration : IEntityTypeConfiguration<ExamAnswer>
{
    public void Configure(EntityTypeBuilder<ExamAnswer> builder)
    {
        builder.ToTable("exam_answers");

        builder.HasKey(ea => ea.Id);

        builder.Property(ea => ea.Answer).HasMaxLength(5000);
        builder.Property(ea => ea.PointsEarned).HasPrecision(5, 2).HasDefaultValue(0m);

        builder.HasIndex(ea => new { ea.UserId, ea.ExamId, ea.AttemptNumber });
        builder.HasIndex(ea => new { ea.ExamId, ea.AttemptNumber });

        builder.HasOne(ea => ea.Question)
            .WithMany()
            .HasForeignKey(ea => ea.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ea => ea.Exam)
            .WithMany()
            .HasForeignKey(ea => ea.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
