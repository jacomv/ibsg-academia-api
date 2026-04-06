using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Application.Common.Email;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academia.Application.Events.Handlers;

public class ExamPassedNotificationHandler : INotificationHandler<ExamPassedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<ExamPassedNotificationHandler> _logger;

    public ExamPassedNotificationHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        ILogger<ExamPassedNotificationHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(ExamPassedEvent notification, CancellationToken cancellationToken)
    {
        // In-platform notification
        var notif = new Notification(
            userId: notification.UserId,
            type: "exam_passed",
            title: "¡Examen aprobado! 🎉",
            message: $"Aprobaste \"{notification.ExamTitle}\" con {notification.TotalScore}%."
        );
        await _context.Notifications.AddAsync(notif, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Email notification
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == notification.UserId, cancellationToken);

        if (user is not null)
        {
            try
            {
                await _emailService.SendAsync(new EmailMessage(
                    To: user.Email,
                    ToName: user.FullName,
                    Subject: $"¡Aprobaste el examen! — {notification.ExamTitle}",
                    HtmlBody: EmailTemplates.ExamPassed(user.FirstName, notification.ExamTitle, notification.TotalScore)
                ), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not send exam passed email to {Email}", user.Email);
            }
        }
    }
}
