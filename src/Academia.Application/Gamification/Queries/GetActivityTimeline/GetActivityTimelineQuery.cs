using Academia.Application.Gamification.Dtos;
using MediatR;

namespace Academia.Application.Gamification.Queries.GetActivityTimeline;

public record GetActivityTimelineQuery(int Days = 14) : IRequest<ActivityTimelineDto>;
