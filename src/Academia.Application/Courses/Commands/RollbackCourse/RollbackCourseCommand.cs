using MediatR;

namespace Academia.Application.Courses.Commands.RollbackCourse;

public record RollbackCourseCommand(Guid CourseId, Guid VersionId) : IRequest;
