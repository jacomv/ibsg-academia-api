using Academia.Application.Common.Interfaces;
using Academia.Application.Gamification.Dtos;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Gamification.Queries.GetLeaderboard;

public class GetLeaderboardQueryHandler : IRequestHandler<GetLeaderboardQuery, LeaderboardDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetLeaderboardQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<LeaderboardDto> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id;

        // Monday of the current week
        var today = DateTime.UtcNow.Date;
        var daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var weekStart = today.AddDays(-daysSinceMonday);

        // Aggregate weekly points per user
        var weeklyTotals = await _context.PointTransactions
            .Where(t => t.CreatedAt >= weekStart)
            .GroupBy(t => t.UserId)
            .Select(g => new { UserId = g.Key, WeeklyPoints = g.Sum(t => t.Points) })
            .OrderByDescending(x => x.WeeklyPoints)
            .Take(10)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Get user details for those top users
        var userIds = weeklyTotals.Select(x => x.UserId).ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .AsNoTracking()
            .Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName, u.Avatar })
            .ToListAsync(cancellationToken);

        var userMap = users.ToDictionary(u => u.Id);

        var topUsers = weeklyTotals
            .Select((entry, index) =>
            {
                var user = userMap.TryGetValue(entry.UserId, out var u) ? u : null;
                return new LeaderboardEntryDto(
                    index + 1,
                    entry.UserId,
                    user?.FullName ?? "Unknown",
                    user?.Avatar,
                    entry.WeeklyPoints);
            })
            .ToList();

        // Find my rank and my weekly points
        var myEntry = topUsers.FirstOrDefault(e => e.UserId == userId);
        int myRank;
        int myWeeklyPoints;

        if (myEntry is not null)
        {
            myRank = myEntry.Rank;
            myWeeklyPoints = myEntry.WeeklyPoints;
        }
        else
        {
            // User not in top 10 — calculate actual rank
            myWeeklyPoints = await _context.PointTransactions
                .Where(t => t.UserId == userId && t.CreatedAt >= weekStart)
                .SumAsync(t => t.Points, cancellationToken);

            var higherCount = await _context.PointTransactions
                .Where(t => t.CreatedAt >= weekStart)
                .GroupBy(t => t.UserId)
                .Select(g => new { UserId = g.Key, Total = g.Sum(t => t.Points) })
                .CountAsync(x => x.Total > myWeeklyPoints, cancellationToken);

            myRank = higherCount + 1;
        }

        return new LeaderboardDto(myRank, myWeeklyPoints, topUsers);
    }
}
