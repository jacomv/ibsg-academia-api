using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Prerequisites.Commands.AddPrerequisite;

public class AddPrerequisiteCommandHandler : IRequestHandler<AddPrerequisiteCommand>
{
    private readonly IApplicationDbContext _context;

    public AddPrerequisiteCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(AddPrerequisiteCommand request, CancellationToken cancellationToken)
    {
        if (request.CourseId == request.PrerequisiteCourseId)
            throw new ValidationException(new[] { "A course cannot be its own prerequisite." });

        var courseExists = await _context.Courses
            .AnyAsync(c => c.Id == request.CourseId, cancellationToken);
        if (!courseExists)
            throw new NotFoundException("Course", request.CourseId);

        var prereqExists = await _context.Courses
            .AnyAsync(c => c.Id == request.PrerequisiteCourseId, cancellationToken);
        if (!prereqExists)
            throw new NotFoundException("Prerequisite course", request.PrerequisiteCourseId);

        var alreadyExists = await _context.CoursePrerequisites
            .AnyAsync(p =>
                p.CourseId == request.CourseId &&
                p.PrerequisiteCourseId == request.PrerequisiteCourseId, cancellationToken);
        if (alreadyExists)
            throw new ValidationException(new[] { "This prerequisite already exists." });

        // Cycle detection: check if adding this would create a circular dependency
        if (await WouldCreateCycleAsync(request.CourseId, request.PrerequisiteCourseId, cancellationToken))
            throw new ValidationException(new[] { "Adding this prerequisite would create a circular dependency." });

        var prerequisite = new CoursePrerequisite(request.CourseId, request.PrerequisiteCourseId);
        await _context.CoursePrerequisites.AddAsync(prerequisite, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> WouldCreateCycleAsync(
        Guid courseId, Guid prereqCourseId, CancellationToken ct)
    {
        // BFS: starting from prereqCourseId, check if we can reach courseId
        // through the existing prerequisite chain
        var allPrereqs = await _context.CoursePrerequisites
            .AsNoTracking()
            .ToListAsync(ct);

        var visited = new HashSet<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(courseId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == prereqCourseId) continue; // skip the direct link being added

            if (!visited.Add(current)) continue;

            // What courses require 'current' as a prerequisite?
            // (i.e., who depends on current → if prereqCourse depends on course, that's a cycle)
            var dependents = allPrereqs
                .Where(p => p.PrerequisiteCourseId == current)
                .Select(p => p.CourseId);

            foreach (var dep in dependents)
            {
                if (dep == prereqCourseId) return true;
                queue.Enqueue(dep);
            }
        }

        // Also check: does prereqCourse already (transitively) require courseId?
        visited.Clear();
        queue.Enqueue(prereqCourseId);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!visited.Add(current)) continue;

            var prereqs = allPrereqs
                .Where(p => p.CourseId == current)
                .Select(p => p.PrerequisiteCourseId);

            foreach (var p in prereqs)
            {
                if (p == courseId) return true;
                queue.Enqueue(p);
            }
        }

        return false;
    }
}
