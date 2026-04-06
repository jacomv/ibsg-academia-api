using Academia.Application.Exams.Dtos;
using MediatR;

namespace Academia.Application.Exams.Commands.SubmitExam;

public record SubmitExamCommand(
    Guid ExamId,
    List<AnswerInput> Answers,
    DateTime StartedAt
) : IRequest<ExamResultDto>;
