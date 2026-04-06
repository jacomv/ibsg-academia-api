using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Enrollments.Dtos;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Enrollments.Commands.ForceEnroll;

public class ForceEnrollCommandHandler : IRequestHandler<ForceEnrollCommand, EnrollmentDto>
{
    private readonly IApplicationDbContext _context;

    public ForceEnrollCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<EnrollmentDto> Handle(ForceEnrollCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists) throw new NotFoundException("User", request.UserId);

        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
        if (course is null) throw new NotFoundException("Course", request.CourseId);

        // Cancel any existing enrollment before creating a new active one
        var existing = await _context.Enrollments
            .Where(e => e.UserId == request.UserId && e.CourseId == request.CourseId)
            .ToListAsync(cancellationToken);

        foreach (var e in existing)
            e.Cancel();

        // Admin enrollments are always Active
        var enrollment = new Enrollment(
            request.UserId, course.Id, EnrollmentStatus.Active,
            request.ExpiresAt, request.Notes);

        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new EnrollmentDto(
            enrollment.Id, enrollment.UserId, course.Id,
            course.Title, course.Image, enrollment.Status,
            enrollment.EnrolledAt, enrollment.ExpiresAt);
    }
}
