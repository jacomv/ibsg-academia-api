using MediatR;

namespace Academia.Application.Events;

/// <summary>Published when a student completes all lessons in a course.</summary>
public record CourseCompletedEvent(
    Guid UserId,
    Guid CourseId,
    string CourseTitle
) : INotification;
