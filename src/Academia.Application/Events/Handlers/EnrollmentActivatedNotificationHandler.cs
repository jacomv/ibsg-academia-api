using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Application.Common.Email;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academia.Application.Events.Handlers;

public class EnrollmentActivatedNotificationHandler : INotificationHandler<EnrollmentActivatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<EnrollmentActivatedNotificationHandler> _logger;

    public EnrollmentActivatedNotificationHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        ILogger<EnrollmentActivatedNotificationHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(EnrollmentActivatedEvent notification, CancellationToken cancellationToken)
    {
        var notif = new Notification(
            userId: notification.UserId,
            type: "enrollment_activated",
            title: "¡Inscripción activada! ✅",
            message: $"Tu inscripción en \"{notification.CourseTitle}\" está activa. ¡Comienza a aprender!"
        );
        await _context.Notifications.AddAsync(notif, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == notification.UserId, cancellationToken);

        if (user is not null)
        {
            try
            {
                await _emailService.SendAsync(new EmailMessage(
                    To: user.Email,
                    ToName: user.FullName,
                    Subject: $"Inscripción activada — {notification.CourseTitle}",
                    HtmlBody: EmailTemplates.EnrollmentActivated(user.FirstName, notification.CourseTitle)
                ), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not send enrollment activated email to {Email}", user.Email);
            }
        }
    }
}
