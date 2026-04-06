using Academia.Application.Common.Models;
using Academia.Application.Enrollments.Dtos;
using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Enrollments.Queries.GetAllEnrollments;

public record GetAllEnrollmentsQuery(
    int Page = 1,
    int PageSize = 20,
    EnrollmentStatus? Status = null,
    Guid? UserId = null,
    Guid? CourseId = null
) : IRequest<PagedResult<EnrollmentDetailDto>>;
