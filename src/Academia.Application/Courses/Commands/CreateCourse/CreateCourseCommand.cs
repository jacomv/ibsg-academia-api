using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Courses.Commands.CreateCourse;

public record CreateCourseCommand(
    string Title,
    string? Description,
    string? Image,
    CourseStatus Status,
    AccessType AccessType,
    decimal? Price,
    int? EstimatedDuration,
    Guid? TeacherId
) : IRequest<Guid>;
