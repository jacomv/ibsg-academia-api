using Academia.Domain.Common;
using Academia.Domain.Enums;

namespace Academia.Domain.Entities;

public class Course : BaseEntity
{
    private readonly List<Chapter> _chapters = new();
    private readonly List<Enrollment> _enrollments = new();
    private readonly List<LearningPathCourse> _learningPaths = new();

    private Course() { }

    public Course(string title, string? description, CourseStatus status, AccessType accessType,
        decimal? price, int? estimatedDuration, Guid? teacherId)
    {
        Title = title;
        Description = description;
        Status = status;
        AccessType = accessType;
        Price = price;
        EstimatedDuration = estimatedDuration;
        TeacherId = teacherId;
    }

    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? Image { get; private set; }
    public CourseStatus Status { get; private set; }
    public AccessType AccessType { get; private set; }
    public decimal? Price { get; private set; }
    public int? EstimatedDuration { get; private set; }
    public Guid? TeacherId { get; private set; }
    public DateTime? PublishAt { get; private set; }
    public DateTime? UnpublishAt { get; private set; }

    public User? Teacher { get; private set; }
    public IReadOnlyCollection<Chapter> Chapters => _chapters.AsReadOnly();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
    public IReadOnlyCollection<LearningPathCourse> LearningPaths => _learningPaths.AsReadOnly();

    public void Update(string title, string? description, string? image, CourseStatus status,
        AccessType accessType, decimal? price, int? estimatedDuration, Guid? teacherId)
    {
        Title = title;
        Description = description;
        Image = image;
        Status = status;
        AccessType = accessType;
        Price = price;
        EstimatedDuration = estimatedDuration;
        TeacherId = teacherId;
    }

    public void SetImage(string imagePath) => Image = imagePath;
    public void Archive() => Status = CourseStatus.Archived;
    public void Publish() => Status = CourseStatus.Published;

    public void SubmitForReview()
    {
        if (Status != CourseStatus.Draft)
            throw new InvalidOperationException("Only draft courses can be submitted for review.");
        Status = CourseStatus.InReview;
    }

    public void Approve()
    {
        if (Status != CourseStatus.InReview)
            throw new InvalidOperationException("Only courses in review can be approved.");
        Status = CourseStatus.Approved;
    }

    public void ReturnToDraft()
    {
        if (Status is not (CourseStatus.InReview or CourseStatus.Approved))
            throw new InvalidOperationException("Only courses in review or approved can be returned to draft.");
        Status = CourseStatus.Draft;
    }

    public void Schedule(DateTime publishAt, DateTime? unpublishAt)
    {
        if (Status != CourseStatus.Approved)
            throw new InvalidOperationException("Only approved courses can be scheduled.");
        PublishAt = publishAt;
        UnpublishAt = unpublishAt;
    }
}
