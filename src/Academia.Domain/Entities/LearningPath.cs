using Academia.Domain.Common;
using Academia.Domain.Enums;

namespace Academia.Domain.Entities;

public class LearningPath : BaseEntity
{
    private readonly List<LearningPathCourse> _courses = new();

    private LearningPath() { }

    public LearningPath(string name, string? description, CourseStatus status,
        AccessType accessType, decimal? price, int globalOrder)
    {
        Name = name;
        Description = description;
        Status = status;
        AccessType = accessType;
        Price = price;
        GlobalOrder = globalOrder;
    }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? Image { get; private set; }
    public CourseStatus Status { get; private set; }
    public AccessType AccessType { get; private set; }
    public decimal? Price { get; private set; }
    public int GlobalOrder { get; private set; }

    public IReadOnlyCollection<LearningPathCourse> Courses => _courses.AsReadOnly();

    public void Update(string name, string? description, string? image, CourseStatus status,
        AccessType accessType, decimal? price, int globalOrder)
    {
        Name = name;
        Description = description;
        Image = image;
        Status = status;
        AccessType = accessType;
        Price = price;
        GlobalOrder = globalOrder;
    }

    public void Archive() => Status = CourseStatus.Archived;
}
