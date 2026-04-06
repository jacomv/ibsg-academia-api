using Academia.Application.Common.Interfaces;
using Academia.Application.Gamification.Dtos;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Gamification.Queries.GetStreak;

public class GetStreakQueryHandler : IRequestHandler<GetStreakQuery, StreakDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetStreakQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<StreakDto> Handle(GetStreakQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id;

        var streak = await _context.UserStreaks
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

        // Get dates of all point transactions for this user (for week history calculation)
        var transactionDates = await _context.PointTransactions
            .Where(t => t.UserId == userId)
            .AsNoTracking()
            .Select(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        // Build week history: last 8 weeks (index 0 = oldest, index 7 = this week)
        var today = DateTime.UtcNow.Date;
        var daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var currentWeekStart = today.AddDays(-daysSinceMonday);

        var weekHistory = new List<bool>();
        for (int i = 7; i >= 0; i--)
        {
            var weekStart = currentWeekStart.AddDays(-7 * i);
            var weekEnd = weekStart.AddDays(7);
            var hasActivity = transactionDates.Any(d => d >= weekStart && d < weekEnd);
            weekHistory.Add(hasActivity);
        }

        // Check if active this week
        var isActiveThisWeek = weekHistory[^1];

        return new StreakDto(
            streak?.CurrentStreak ?? 0,
            streak?.LongestStreak ?? 0,
            streak?.LastActivityDate,
            isActiveThisWeek,
            weekHistory);
    }
}
