using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Queries.GetCourseVersions;

public class GetCourseVersionsQueryHandler : IRequestHandler<GetCourseVersionsQuery, List<CourseVersionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCourseVersionsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CourseVersionDto>> Handle(
        GetCourseVersionsQuery request, CancellationToken cancellationToken)
    {
        var courseExists = await _context.Courses
            .AnyAsync(c => c.Id == request.CourseId, cancellationToken);

        if (!courseExists)
            throw new NotFoundException("Course", request.CourseId);

        return await _context.CourseVersions
            .Where(v => v.CourseId == request.CourseId)
            .Include(v => v.Author)
            .OrderByDescending(v => v.VersionNumber)
            .Select(v => new CourseVersionDto(
                v.Id,
                v.VersionNumber,
                v.Reason,
                v.AuthorId,
                v.Author.FirstName + " " + v.Author.LastName,
                v.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
