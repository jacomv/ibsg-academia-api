using MediatR;

namespace Academia.Application.Prerequisites.Commands.AddPrerequisite;

public record AddPrerequisiteCommand(Guid CourseId, Guid PrerequisiteCourseId) : IRequest;
