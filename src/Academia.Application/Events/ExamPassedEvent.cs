using MediatR;

namespace Academia.Application.Events;

/// <summary>Published when a student passes an exam (auto-graded, score >= passing).</summary>
public record ExamPassedEvent(
    Guid UserId,
    Guid ExamId,
    string ExamTitle,
    decimal TotalScore
) : INotification;
