using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Queries.GetEditorialReviews;

public class GetEditorialReviewsQueryHandler
    : IRequestHandler<GetEditorialReviewsQuery, List<EditorialReviewDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEditorialReviewsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<EditorialReviewDto>> Handle(
        GetEditorialReviewsQuery request, CancellationToken cancellationToken)
    {
        return await _context.EditorialReviews
            .Where(r => r.CourseId == request.CourseId)
            .Include(r => r.Reviewer)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new EditorialReviewDto(
                r.Id,
                r.ReviewerId,
                r.Reviewer.FirstName + " " + r.Reviewer.LastName,
                r.Decision,
                r.Comment,
                r.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
