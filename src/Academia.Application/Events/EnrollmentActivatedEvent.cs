using MediatR;

namespace Academia.Application.Events;

/// <summary>Published when an enrollment transitions to Active status.</summary>
public record EnrollmentActivatedEvent(
    Guid UserId,
    Guid CourseId,
    string CourseTitle
) : INotification;
