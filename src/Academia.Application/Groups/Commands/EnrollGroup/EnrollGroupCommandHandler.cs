using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Groups.Commands.EnrollGroup;

public class EnrollGroupCommandHandler : IRequestHandler<EnrollGroupCommand, int>
{
    private readonly IApplicationDbContext _context;

    public EnrollGroupCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(EnrollGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

        if (group is null)
            throw new NotFoundException("Group", request.GroupId);

        var courseExists = await _context.Courses
            .AnyAsync(c => c.Id == request.CourseId, cancellationToken);
        if (!courseExists)
            throw new NotFoundException("Course", request.CourseId);

        var memberUserIds = group.Members.Select(m => m.UserId).ToList();

        if (!memberUserIds.Any()) return 0;

        // Cancel any existing enrollment for each member
        var existingEnrollments = await _context.Enrollments
            .Where(e => memberUserIds.Contains(e.UserId) && e.CourseId == request.CourseId)
            .ToListAsync(cancellationToken);

        foreach (var enrollment in existingEnrollments)
            enrollment.Cancel();

        // Create new active enrollments (reusing ForceEnroll logic)
        int enrolled = 0;
        foreach (var userId in memberUserIds)
        {
            var newEnrollment = new Enrollment(
                userId, request.CourseId, EnrollmentStatus.Active,
                expiresAt: null, notes: $"Group enrollment: {group.Name}");
            await _context.Enrollments.AddAsync(newEnrollment, cancellationToken);
            enrolled++;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return enrolled;
    }
}
