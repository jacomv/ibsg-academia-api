using Academia.Application.LearningPaths.Dtos;
using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.LearningPaths.Queries.GetLearningPaths;

public record GetLearningPathsQuery(
    CourseStatus? Status = null,
    bool PublicOnly = false
) : IRequest<List<LearningPathListDto>>;
