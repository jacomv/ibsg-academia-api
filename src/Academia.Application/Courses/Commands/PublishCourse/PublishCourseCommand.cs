using MediatR;

namespace Academia.Application.Courses.Commands.PublishCourse;

public record PublishCourseCommand(Guid CourseId) : IRequest;
