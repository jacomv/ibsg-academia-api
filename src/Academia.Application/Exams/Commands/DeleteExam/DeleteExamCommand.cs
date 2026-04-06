using MediatR;

namespace Academia.Application.Exams.Commands.DeleteExam;

public record DeleteExamCommand(Guid Id) : IRequest;
