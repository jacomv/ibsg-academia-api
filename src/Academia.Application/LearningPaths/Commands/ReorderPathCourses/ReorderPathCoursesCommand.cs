using MediatR;

namespace Academia.Application.LearningPaths.Commands.ReorderPathCourses;

public record CourseOrderItem(Guid CourseId, int Order);

public record ReorderPathCoursesCommand(
    Guid LearningPathId,
    List<CourseOrderItem> Courses
) : IRequest;
