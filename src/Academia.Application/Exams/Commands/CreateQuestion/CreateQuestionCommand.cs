using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Exams.Commands.CreateQuestion;

public record CreateQuestionCommand(
    Guid ExamId,
    QuestionType Type,
    string Text,
    List<string> Options,
    string? CorrectAnswer,
    decimal Points,
    int Order
) : IRequest<Guid>;
