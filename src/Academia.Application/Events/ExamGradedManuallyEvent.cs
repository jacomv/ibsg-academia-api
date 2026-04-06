using MediatR;

namespace Academia.Application.Events;

/// <summary>Published when a teacher completes manual grading of an exam.</summary>
public record ExamGradedManuallyEvent(
    Guid UserId,
    Guid ExamId,
    string ExamTitle,
    decimal TotalScore,
    bool IsPassed
) : INotification;
