using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.LearningPaths.Commands.AddCourseToPath;

public class AddCourseToPathCommandHandler : IRequestHandler<AddCourseToPathCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddCourseToPathCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddCourseToPathCommand request, CancellationToken cancellationToken)
    {
        var pathExists = await _context.LearningPaths
            .AnyAsync(lp => lp.Id == request.LearningPathId, cancellationToken);
        if (!pathExists) throw new NotFoundException("LearningPath", request.LearningPathId);

        var courseExists = await _context.Courses
            .AnyAsync(c => c.Id == request.CourseId, cancellationToken);
        if (!courseExists) throw new NotFoundException("Course", request.CourseId);

        var alreadyAdded = await _context.LearningPathCourses
            .AnyAsync(lpc =>
                lpc.LearningPathId == request.LearningPathId &&
                lpc.CourseId == request.CourseId, cancellationToken);

        if (alreadyAdded)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["courseId"] = ["This course is already in the learning path."]
            });

        // Auto-assign next order
        var maxOrder = await _context.LearningPathCourses
            .Where(lpc => lpc.LearningPathId == request.LearningPathId)
            .MaxAsync(lpc => (int?)lpc.Order, cancellationToken) ?? 0;

        var entry = new LearningPathCourse(
            request.LearningPathId, request.CourseId,
            maxOrder + 1, request.IsRequired);

        await _context.LearningPathCourses.AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Id;
    }
}
