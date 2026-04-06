using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Exams.Commands.UpdateQuestion;

public record UpdateQuestionCommand(
    Guid Id, QuestionType Type, string Text, List<string> Options,
    string? CorrectAnswer, decimal Points, int Order
) : IRequest;
