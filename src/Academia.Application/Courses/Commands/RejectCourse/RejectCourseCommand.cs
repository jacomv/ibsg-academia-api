using MediatR;

namespace Academia.Application.Courses.Commands.RejectCourse;

public record RejectCourseCommand(Guid CourseId, string Comment) : IRequest;
