using Academia.Application.Common.Email;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Academia.Application.Events.Handlers;

public class CourseCompletedHandler : INotificationHandler<CourseCompletedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ICertificateGenerator _generator;
    private readonly IEmailService _emailService;
    private readonly ILogger<CourseCompletedHandler> _logger;

    public CourseCompletedHandler(
        IApplicationDbContext context,
        ICertificateGenerator generator,
        IEmailService emailService,
        ILogger<CourseCompletedHandler> logger)
    {
        _context = context;
        _generator = generator;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(CourseCompletedEvent notification, CancellationToken cancellationToken)
    {
        // Avoid duplicate certificates
        var alreadyIssued = await _context.Certificates.AnyAsync(c =>
            c.UserId == notification.UserId &&
            c.CourseId == notification.CourseId, cancellationToken);

        if (alreadyIssued) return;

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == notification.UserId, cancellationToken);
        if (user is null) return;

        // Generate unique certificate number: IBSG-YYYYMMDD-XXXXXXXX
        var certNumber = $"IBSG-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

        // Generate PDF
        var pdfBytes = _generator.Generate(new CertificateData(
            StudentFullName: user.FullName,
            CourseTitle: notification.CourseTitle,
            CertificateNumber: certNumber,
            IssuedAt: DateTime.UtcNow
        ));

        // Save certificate record
        var certificate = new Certificate(notification.UserId, notification.CourseId, certNumber);
        await _context.Certificates.AddAsync(certificate, cancellationToken);

        // In-platform notification
        var notif = new Notification(
            userId: notification.UserId,
            type: "course_completed",
            title: "🏆 ¡Curso completado!",
            message: $"Completaste \"{notification.CourseTitle}\". Tu certificado está listo."
        );
        await _context.Notifications.AddAsync(notif, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Email with PDF attachment
        try
        {
            await _emailService.SendAsync(new EmailMessage(
                To: user.Email,
                ToName: user.FullName,
                Subject: $"🏆 ¡Completaste el curso! — {notification.CourseTitle}",
                HtmlBody: EmailTemplates.Certificate(user.FirstName, notification.CourseTitle, certNumber),
                Attachment: pdfBytes,
                AttachmentFileName: $"Certificado_{notification.CourseTitle.Replace(" ", "_")}.pdf"
            ), cancellationToken);

            _logger.LogInformation("Certificate issued: {CertNumber} for user {UserId}", certNumber, notification.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not send certificate email to {Email}", user.Email);
        }
    }
}
