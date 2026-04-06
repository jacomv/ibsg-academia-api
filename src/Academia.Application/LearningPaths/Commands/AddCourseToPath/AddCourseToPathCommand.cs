using MediatR;

namespace Academia.Application.LearningPaths.Commands.AddCourseToPath;

public record AddCourseToPathCommand(Guid LearningPathId, Guid CourseId, bool IsRequired) : IRequest<Guid>;
