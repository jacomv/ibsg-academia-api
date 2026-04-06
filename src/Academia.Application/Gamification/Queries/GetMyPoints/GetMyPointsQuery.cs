using Academia.Application.Gamification.Dtos;
using MediatR;

namespace Academia.Application.Gamification.Queries.GetMyPoints;

public record GetMyPointsQuery : IRequest<MyPointsDto>;
