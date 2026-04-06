using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.LearningPaths.Commands.CreateLearningPath;

public record CreateLearningPathCommand(
    string Name, string? Description, string? Image,
    CourseStatus Status, AccessType AccessType,
    decimal? Price, int GlobalOrder
) : IRequest<Guid>;
