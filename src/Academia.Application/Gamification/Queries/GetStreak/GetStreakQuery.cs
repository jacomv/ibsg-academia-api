using Academia.Application.Gamification.Dtos;
using MediatR;

namespace Academia.Application.Gamification.Queries.GetStreak;

public record GetStreakQuery : IRequest<StreakDto>;
