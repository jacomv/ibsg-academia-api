using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("questions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(q => q.Text).IsRequired().HasMaxLength(2000);
        builder.Property(q => q.CorrectAnswer).HasMaxLength(1000);
        builder.Property(q => q.Points).HasPrecision(5, 2);

        // Store options array as jsonb for efficient storage and querying
        builder.Property(q => q.Options)
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[]'::jsonb");

        builder.HasIndex(q => new { q.ExamId, q.Order });
    }
}
