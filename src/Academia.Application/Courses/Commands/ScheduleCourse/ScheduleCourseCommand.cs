using MediatR;

namespace Academia.Application.Courses.Commands.ScheduleCourse;

public record ScheduleCourseCommand(Guid CourseId, DateTime PublishAt, DateTime? UnpublishAt) : IRequest;
