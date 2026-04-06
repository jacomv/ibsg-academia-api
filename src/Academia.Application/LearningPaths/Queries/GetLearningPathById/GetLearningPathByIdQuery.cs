using Academia.Application.LearningPaths.Dtos;
using MediatR;

namespace Academia.Application.LearningPaths.Queries.GetLearningPathById;

public record GetLearningPathByIdQuery(Guid Id) : IRequest<LearningPathDetailDto>;
