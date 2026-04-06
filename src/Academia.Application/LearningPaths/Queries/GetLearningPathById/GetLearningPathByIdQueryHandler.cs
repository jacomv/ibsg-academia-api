using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.LearningPaths.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.LearningPaths.Queries.GetLearningPathById;

public class GetLearningPathByIdQueryHandler
    : IRequestHandler<GetLearningPathByIdQuery, LearningPathDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetLearningPathByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<LearningPathDetailDto> Handle(
        GetLearningPathByIdQuery request, CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .Include(lp => lp.Courses.OrderBy(c => c.Order))
                .ThenInclude(lpc => lpc.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(lp => lp.Id == request.Id, cancellationToken);

        if (path is null)
            throw new NotFoundException("LearningPath", request.Id);

        return new LearningPathDetailDto(
            Id: path.Id,
            Name: path.Name,
            Description: path.Description,
            Image: path.Image,
            Status: path.Status,
            AccessType: path.AccessType,
            Price: path.Price,
            GlobalOrder: path.GlobalOrder,
            Courses: path.Courses.Select(lpc => new PathCourseDto(
                LearningPathCourseId: lpc.Id,
                CourseId: lpc.CourseId,
                Title: lpc.Course.Title,
                Image: lpc.Course.Image,
                Status: lpc.Course.Status,
                Order: lpc.Order,
                IsRequired: lpc.IsRequired,
                EstimatedDuration: lpc.Course.EstimatedDuration ?? 0
            )).ToList(),
            CreatedAt: path.CreatedAt
        );
    }
}
