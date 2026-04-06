using Academia.Application.Common.Interfaces;
using Academia.Application.Gamification.Dtos;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Gamification.Queries.GetActivityTimeline;

public class GetActivityTimelineQueryHandler : IRequestHandler<GetActivityTimelineQuery, ActivityTimelineDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetActivityTimelineQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ActivityTimelineDto> Handle(GetActivityTimelineQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id;
        var days = Math.Clamp(request.Days, 1, 90);
        var since = DateTime.UtcNow.Date.AddDays(-(days - 1));

        // Lessons completed per day
        var completedByDay = await _context.UserProgress
            .Where(p => p.UserId == userId
                && p.Status == ProgressStatus.Completed
                && p.CompletedAt.HasValue
                && p.CompletedAt >= since)
            .GroupBy(p => p.CompletedAt!.Value.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Points earned per day
        var pointsByDay = await _context.PointTransactions
            .Where(t => t.UserId == userId && t.CreatedAt >= since)
            .GroupBy(t => t.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Points = g.Sum(t => t.Points) })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var completedMap = completedByDay.ToDictionary(x => x.Date, x => x.Count);
        var pointsMap = pointsByDay.ToDictionary(x => x.Date, x => x.Points);

        // Build a continuous series for every day in range
        var dailyList = Enumerable.Range(0, days)
            .Select(i =>
            {
                var date = since.AddDays(i);
                completedMap.TryGetValue(date, out var lessons);
                pointsMap.TryGetValue(date, out var pts);
                return new DailyActivityDto(date, lessons, pts);
            })
            .ToList();

        return new ActivityTimelineDto(dailyList);
    }
}
