using Academia.Application.Common.Interfaces;
using Academia.Application.LearningPaths.Dtos;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.LearningPaths.Queries.GetLearningPaths;

public class GetLearningPathsQueryHandler
    : IRequestHandler<GetLearningPathsQuery, List<LearningPathListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLearningPathsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<LearningPathListDto>> Handle(
        GetLearningPathsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.LearningPaths
            .Include(lp => lp.Courses)
            .AsNoTracking()
            .AsQueryable();

        if (request.PublicOnly)
            query = query.Where(lp => lp.Status == CourseStatus.Published);
        else if (request.Status.HasValue)
            query = query.Where(lp => lp.Status == request.Status.Value);

        return await query
            .OrderBy(lp => lp.GlobalOrder)
            .Select(lp => new LearningPathListDto(
                lp.Id, lp.Name, lp.Description, lp.Image,
                lp.Status, lp.AccessType, lp.Price, lp.GlobalOrder,
                lp.Courses.Count, lp.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
