using Academia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academia.Infrastructure.Persistence.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("certificates");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.CertificateNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.CertificateNumber).IsUnique();
        builder.HasIndex(c => new { c.UserId, c.CourseId }).IsUnique();
        builder.HasOne(c => c.User).WithMany()
            .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(c => c.Course).WithMany()
            .HasForeignKey(c => c.CourseId).OnDelete(DeleteBehavior.Cascade);
    }
}
