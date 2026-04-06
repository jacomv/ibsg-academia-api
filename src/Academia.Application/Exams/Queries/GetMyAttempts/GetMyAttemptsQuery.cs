using Academia.Application.Exams.Dtos;
using MediatR;

namespace Academia.Application.Exams.Queries.GetMyAttempts;

public record GetMyAttemptsQuery(Guid ExamId) : IRequest<List<AttemptSummaryDto>>;
