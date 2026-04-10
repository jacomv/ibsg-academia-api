using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class StudentNoteConfiguration : IEntityTypeConfiguration<StudentNote>
{
    public void Configure(EntityTypeBuilder<StudentNote> builder)
    {
        builder.ToTable("student_notes");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Content).IsRequired();

        builder.HasIndex(n => new { n.UserId, n.LessonId });

        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Lesson)
            .WithMany()
            .HasForeignKey(n => n.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
