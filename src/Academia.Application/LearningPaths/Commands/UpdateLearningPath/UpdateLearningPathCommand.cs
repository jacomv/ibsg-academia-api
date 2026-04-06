using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.LearningPaths.Commands.UpdateLearningPath;

public record UpdateLearningPathCommand(
    Guid Id, string Name, string? Description, string? Image,
    CourseStatus Status, AccessType AccessType, decimal? Price, int GlobalOrder
) : IRequest;
