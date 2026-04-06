using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class LearningPathCourse : BaseEntity
{
    private LearningPathCourse() { }

    public LearningPathCourse(Guid learningPathId, Guid courseId, int order, bool isRequired)
    {
        LearningPathId = learningPathId;
        CourseId = courseId;
        Order = order;
        IsRequired = isRequired;
    }

    public Guid LearningPathId { get; private set; }
    public Guid CourseId { get; private set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; }

    public LearningPath LearningPath { get; private set; } = default!;
    public Course Course { get; private set; } = default!;
}
