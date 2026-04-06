using Academia.Domain.Common;
using Academia.Domain.Enums;

namespace Academia.Domain.Entities;

public class Enrollment : BaseEntity
{
    private Enrollment() { }

    public Enrollment(Guid userId, Guid courseId, EnrollmentStatus status,
        DateTime? expiresAt = null, string? notes = null)
    {
        UserId = userId;
        CourseId = courseId;
        Status = status;
        EnrolledAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        Notes = notes;
    }

    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public DateTime EnrolledAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? Notes { get; private set; }

    public User User { get; private set; } = default!;
    public Course Course { get; private set; } = default!;

    public bool IsActive => Status == EnrollmentStatus.Active &&
        (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);

    public void Activate() => Status = EnrollmentStatus.Active;
    public void Cancel() => Status = EnrollmentStatus.Cancelled;
    public void Expire() => Status = EnrollmentStatus.Expired;
}
