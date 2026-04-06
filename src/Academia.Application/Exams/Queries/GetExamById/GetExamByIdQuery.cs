using Academia.Application.Exams.Dtos;
using MediatR;

namespace Academia.Application.Exams.Queries.GetExamById;

public record GetExamByIdQuery(Guid ExamId) : IRequest<ExamDto>;
