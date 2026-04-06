using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academia.Application.Events.Handlers;

public class CommentCreatedNotificationHandler : INotificationHandler<CommentCreatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CommentCreatedNotificationHandler> _logger;

    public CommentCreatedNotificationHandler(
        IApplicationDbContext context,
        ILogger<CommentCreatedNotificationHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(CommentCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Find the course/lesson teacher and all admins to notify
        var lesson = await _context.Lessons
            .Include(l => l.Chapter)
                .ThenInclude(ch => ch.Course)
                    .ThenInclude(c => c.Teacher)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == notification.LessonId, cancellationToken);

        if (lesson is null) return;

        var recipientIds = new HashSet<Guid>();

        // Add teacher if exists
        if (lesson.Chapter?.Course?.TeacherId.HasValue == true)
            recipientIds.Add(lesson.Chapter.Course.TeacherId.Value);

        // Add all admins
        var adminIds = await _context.Users
            .Where(u => u.Role == UserRole.Administrator)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        foreach (var id in adminIds)
            recipientIds.Add(id);

        // Don't notify the commenter themselves
        recipientIds.Remove(notification.UserId);

        if (!recipientIds.Any()) return;

        var commenter = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == notification.UserId, cancellationToken);

        var commenterName = commenter?.FullName ?? notification.UserEmail;
        var lessonTitle = lesson.Title;

        var notifications = recipientIds.Select(recipientId => new Notification(
            userId: recipientId,
            type: "comment_created",
            title: "Nuevo comentario",
            message: $"{commenterName} comentó en la lección \"{lessonTitle}\"."
        )).ToList();

        await _context.Notifications.AddRangeAsync(notifications, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Comment notification sent to {Count} recipients for lesson {LessonId}",
            notifications.Count, notification.LessonId);
    }
}
