using MediatR;

namespace Academia.Application.Exams.Commands.CreateExam;

public record CreateExamCommand(
    string Title,
    Guid? CourseId,
    Guid? ChapterId,
    decimal PassingScore,
    int MaxAttempts,
    int? TimeLimitMinutes,
    int Order
) : IRequest<Guid>;
