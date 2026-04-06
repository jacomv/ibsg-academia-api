using Academia.Application.Common.Interfaces;
using Academia.Application.Enrollments.Dtos;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Enrollments.Queries.GetMyEnrollments;

public class GetMyEnrollmentsQueryHandler : IRequestHandler<GetMyEnrollmentsQuery, List<EnrollmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMyEnrollmentsQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<EnrollmentDto>> Handle(
        GetMyEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.UserId == _currentUser.Id)
            .OrderByDescending(e => e.EnrolledAt)
            .AsNoTracking()
            .Select(e => new EnrollmentDto(
                e.Id, e.UserId, e.CourseId,
                e.Course.Title, e.Course.Image,
                e.Status, e.EnrolledAt, e.ExpiresAt))
            .ToListAsync(cancellationToken);
    }
}
