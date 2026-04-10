using MediatR;

namespace Academia.Application.Prerequisites.Commands.RemovePrerequisite;

public record RemovePrerequisiteCommand(Guid CourseId, Guid PrerequisiteCourseId) : IRequest;
