using Academia.Application.Gamification.Dtos;
using MediatR;

namespace Academia.Application.Gamification.Queries.GetLeaderboard;

public record GetLeaderboardQuery : IRequest<LeaderboardDto>;
