using Academia.Application.Common.Interfaces;
using Academia.Application.Gamification.Dtos;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Gamification.Queries.GetMyPoints;

public class GetMyPointsQueryHandler : IRequestHandler<GetMyPointsQuery, MyPointsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMyPointsQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<MyPointsDto> Handle(GetMyPointsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id;
        var now = DateTime.UtcNow;

        // Monday of the current week
        var today = now.Date;
        var daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var weekStart = today.AddDays(-daysSinceMonday);

        // First day of the current month
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var transactions = await _context.PointTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalPoints = transactions.Sum(t => t.Points);
        var weeklyPoints = transactions
            .Where(t => t.CreatedAt >= weekStart)
            .Sum(t => t.Points);
        var monthlyPoints = transactions
            .Where(t => t.CreatedAt >= monthStart)
            .Sum(t => t.Points);

        var (level, levelName, pointsToNext) = LevelSystem.Calculate(totalPoints);

        var recentTransactions = transactions
            .Take(10)
            .Select(t => new PointTransactionDto(t.Points, t.Reason, t.CreatedAt))
            .ToList();

        return new MyPointsDto(
            totalPoints,
            level,
            levelName,
            pointsToNext,
            weeklyPoints,
            monthlyPoints,
            recentTransactions);
    }
}
