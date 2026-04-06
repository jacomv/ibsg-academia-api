using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.LearningPaths.Commands.ReorderPathCourses;

public class ReorderPathCoursesCommandHandler : IRequestHandler<ReorderPathCoursesCommand>
{
    private readonly IApplicationDbContext _context;

    public ReorderPathCoursesCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ReorderPathCoursesCommand request, CancellationToken cancellationToken)
    {
        var entries = await _context.LearningPathCourses
            .Where(lpc => lpc.LearningPathId == request.LearningPathId)
            .ToListAsync(cancellationToken);

        foreach (var item in request.Courses)
        {
            var entry = entries.FirstOrDefault(e => e.CourseId == item.CourseId);
            if (entry is not null)
                entry.Order = item.Order;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
