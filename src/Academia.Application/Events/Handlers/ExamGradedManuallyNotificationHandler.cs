using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;

namespace Academia.Application.Events.Handlers;

public class ExamGradedManuallyNotificationHandler : INotificationHandler<ExamGradedManuallyEvent>
{
    private readonly IApplicationDbContext _context;

    public ExamGradedManuallyNotificationHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ExamGradedManuallyEvent notification, CancellationToken cancellationToken)
    {
        var result = notification.IsPassed ? "passed" : "did not pass";
        var notif = new Notification(
            userId: notification.UserId,
            type: "exam_graded",
            title: "Exam graded by teacher",
            message: $"Your exam \"{notification.ExamTitle}\" has been graded. " +
                     $"You scored {notification.TotalScore}% and {result}."
        );

        await _context.Notifications.AddAsync(notif, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
