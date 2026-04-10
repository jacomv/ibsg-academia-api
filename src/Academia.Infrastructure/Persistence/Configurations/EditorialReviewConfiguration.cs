using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class EditorialReviewConfiguration : IEntityTypeConfiguration<EditorialReview>
{
    public void Configure(EntityTypeBuilder<EditorialReview> builder)
    {
        builder.ToTable("editorial_reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Decision).HasConversion<string>().HasMaxLength(50);
        builder.Property(r => r.Comment).HasMaxLength(2000);

        builder.HasIndex(r => r.CourseId);

        builder.HasOne(r => r.Course)
            .WithMany()
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
