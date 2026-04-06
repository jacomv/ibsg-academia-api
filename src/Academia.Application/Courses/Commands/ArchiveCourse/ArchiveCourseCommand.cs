using MediatR;

namespace Academia.Application.Courses.Commands.ArchiveCourse;

public record ArchiveCourseCommand(Guid Id) : IRequest;
