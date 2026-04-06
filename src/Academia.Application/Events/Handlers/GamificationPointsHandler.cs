using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academia.Application.Events.Handlers;

/// <summary>Awards points and updates streak when gamification events fire.</summary>
public class GamificationPointsHandler :
    INotificationHandler<ExamPassedEvent>,
    INotificationHandler<CourseCompletedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GamificationPointsHandler> _logger;

    public GamificationPointsHandler(IApplicationDbContext context, ILogger<GamificationPointsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Exam passed → +100 pts
    public async Task Handle(ExamPassedEvent notification, CancellationToken ct)
    {
        await AwardPoints(notification.UserId, 100, "exam_passed",
            $"Exam passed: {notification.ExamTitle}", notification.ExamId.ToString(), ct);
    }

    // Course completed → +500 pts
    public async Task Handle(CourseCompletedEvent notification, CancellationToken ct)
    {
        await AwardPoints(notification.UserId, 500, "course_completed",
            $"Course completed: {notification.CourseTitle}", notification.CourseId.ToString(), ct);
    }

    private async Task AwardPoints(Guid userId, int points, string reason,
        string displayReason, string? referenceId, CancellationToken ct)
    {
        var transaction = new PointTransaction(userId, points, displayReason, referenceId);
        await _context.PointTransactions.AddAsync(transaction, ct);

        // Update streak
        var streak = await _context.UserStreaks
            .FirstOrDefaultAsync(s => s.UserId == userId, ct);

        if (streak is null)
        {
            streak = new UserStreak(userId);
            await _context.UserStreaks.AddAsync(streak, ct);
        }

        streak.RecordActivity();
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Awarded {Points} pts to {UserId} for {Reason}", points, userId, reason);
    }
}
