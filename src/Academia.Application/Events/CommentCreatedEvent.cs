using MediatR;

namespace Academia.Application.Events;

/// <summary>Published when a student posts a comment on a lesson.</summary>
public record CommentCreatedEvent(
    Guid CommentId,
    Guid LessonId,
    Guid UserId,
    string UserEmail
) : INotification;
