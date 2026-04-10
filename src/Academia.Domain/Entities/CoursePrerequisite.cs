using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class CoursePrerequisite : BaseEntity
{
    private CoursePrerequisite() { }

    public CoursePrerequisite(Guid courseId, Guid prerequisiteCourseId)
    {
        CourseId = courseId;
        PrerequisiteCourseId = prerequisiteCourseId;
    }

    public Guid CourseId { get; private set; }
    public Guid PrerequisiteCourseId { get; private set; }

    public Course Course { get; private set; } = default!;
    public Course PrerequisiteCourse { get; private set; } = default!;
}
