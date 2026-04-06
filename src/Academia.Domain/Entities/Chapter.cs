using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class Chapter : BaseEntity
{
    private readonly List<Lesson> _lessons = new();

    private Chapter() { }

    public Chapter(Guid courseId, string title, string? description, int order, DateTime? availableFrom)
    {
        CourseId = courseId;
        Title = title;
        Description = description;
        Order = order;
        AvailableFrom = availableFrom;
    }

    public Guid CourseId { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public int Order { get; private set; }
    public DateTime? AvailableFrom { get; private set; }

    public Course Course { get; private set; } = default!;
    public Exam? Exam { get; private set; }
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

    public bool IsLocked => AvailableFrom.HasValue && DateTime.UtcNow < AvailableFrom.Value;

    public void Update(string title, string? description, int order, DateTime? availableFrom)
    {
        Title = title;
        Description = description;
        Order = order;
        AvailableFrom = availableFrom;
    }
}
