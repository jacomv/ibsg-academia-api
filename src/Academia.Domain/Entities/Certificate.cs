using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class Certificate : BaseEntity
{
    private Certificate() { }

    public Certificate(Guid userId, Guid courseId, string certificateNumber)
    {
        UserId = userId;
        CourseId = courseId;
        CertificateNumber = certificateNumber;
        IssuedAt = DateTime.UtcNow;
    }

    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }
    public string CertificateNumber { get; private set; } = default!;
    public DateTime IssuedAt { get; private set; }

    public User User { get; private set; } = default!;
    public Course Course { get; private set; } = default!;
}
