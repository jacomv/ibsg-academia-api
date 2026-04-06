using MediatR;

namespace Academia.Application.LearningPaths.Commands.RemoveCourseFromPath;

public record RemoveCourseFromPathCommand(Guid LearningPathId, Guid CourseId) : IRequest;
