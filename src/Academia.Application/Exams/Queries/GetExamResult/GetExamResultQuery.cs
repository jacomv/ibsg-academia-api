using Academia.Application.Exams.Dtos;
using MediatR;

namespace Academia.Application.Exams.Queries.GetExamResult;

public record GetExamResultQuery(Guid ExamId, int AttemptNumber) : IRequest<ExamResultDto>;
