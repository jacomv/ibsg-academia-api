using Academia.Application.Common.Models;
using Academia.Application.Grading.Dtos;
using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Grading.Queries.GetPendingGrades;

public record GetPendingGradesQuery(
    int Page = 1,
    int PageSize = 20,
    GradingStatus? Status = null
) : IRequest<PagedResult<PendingGradeDto>>;
