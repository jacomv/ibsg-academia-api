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
}
