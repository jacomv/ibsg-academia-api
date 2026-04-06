using Academia.Application.Common.Interfaces;
using Academia.Application.Common.Models;
using Academia.Application.Enrollments.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Enrollments.Queries.GetAllEnrollments;

public class GetAllEnrollmentsQueryHandler
    : IRequestHandler<GetAllEnrollmentsQuery, PagedResult<EnrollmentDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllEnrollmentsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<EnrollmentDetailDto>> Handle(
        GetAllEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Enrollments
            .Include(e => e.User)
            .Include(e => e.Course)
            .AsNoTracking()
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(e => e.Status == request.Status.Value);
        if (request.UserId.HasValue)
            query = query.Where(e => e.UserId == request.UserId.Value);
        if (request.CourseId.HasValue)
            query = query.Where(e => e.CourseId == request.CourseId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EnrollmentDetailDto(
                e.Id, e.UserId, e.User.FirstName + " " + e.User.LastName,
                e.User.Email, e.CourseId, e.Course.Title,
                e.Status, e.EnrolledAt, e.ExpiresAt, e.Notes))
            .ToListAsync(cancellationToken);

        return new PagedResult<EnrollmentDetailDto>(items, totalCount, request.Page, request.PageSize);
    }
}
