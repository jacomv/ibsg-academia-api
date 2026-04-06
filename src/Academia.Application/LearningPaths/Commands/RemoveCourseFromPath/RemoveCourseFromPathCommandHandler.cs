using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.LearningPaths.Commands.RemoveCourseFromPath;

public class RemoveCourseFromPathCommandHandler : IRequestHandler<RemoveCourseFromPathCommand>
{
    private readonly IApplicationDbContext _context;

    public RemoveCourseFromPathCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(RemoveCourseFromPathCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.LearningPathCourses
            .FirstOrDefaultAsync(lpc =>
                lpc.LearningPathId == request.LearningPathId &&
                lpc.CourseId == request.CourseId, cancellationToken);

        if (entry is null)
            throw new NotFoundException("LearningPathCourse", $"{request.LearningPathId}/{request.CourseId}");

        _context.LearningPathCourses.Remove(entry);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
