using MediatR;

namespace Academia.Application.Exams.Commands.UpdateExam;

public record UpdateExamCommand(
    Guid Id, string Title, decimal PassingScore,
    int MaxAttempts, int? TimeLimitMinutes, int Order
) : IRequest;
