using MediatR;

namespace Academia.Application.Courses.Commands.ApproveCourse;

public record ApproveCourseCommand(Guid CourseId, string? Comment) : IRequest;
