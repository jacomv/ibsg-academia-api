using Academia.Application.Common.Models;
using Academia.Application.Courses.Dtos;
using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Courses.Queries.GetCourses;

public record GetCoursesQuery(
    int Page = 1,
    int PageSize = 12,
    CourseStatus? Status = null,
    AccessType? AccessType = null,
    Guid? TeacherId = null,
    string? Search = null,
    bool PublicOnly = false
) : IRequest<PagedResult<CourseListDto>>;
