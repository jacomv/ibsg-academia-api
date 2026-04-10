using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        builder.ToTable("bookmarks");

        builder.HasKey(b => b.Id);

        builder.HasIndex(b => new { b.UserId, b.LessonId }).IsUnique();

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Lesson)
            .WithMany()
            .HasForeignKey(b => b.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
