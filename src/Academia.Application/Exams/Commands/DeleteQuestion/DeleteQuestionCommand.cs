using MediatR;

namespace Academia.Application.Exams.Commands.DeleteQuestion;

public record DeleteQuestionCommand(Guid Id) : IRequest;
