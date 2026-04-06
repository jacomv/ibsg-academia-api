using MediatR;

namespace Academia.Application.LearningPaths.Commands.ArchiveLearningPath;

public record ArchiveLearningPathCommand(Guid Id) : IRequest;
