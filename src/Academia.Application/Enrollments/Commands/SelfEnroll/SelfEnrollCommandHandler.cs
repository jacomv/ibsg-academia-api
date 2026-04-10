using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Enrollments.Dtos;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Enrollments.Commands.SelfEnroll;

public class SelfEnrollCommandHandler : IRequestHandler<SelfEnrollCommand, EnrollmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public SelfEnrollCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<EnrollmentDto> Handle(SelfEnrollCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.CourseId);

        if (course.Status != CourseStatus.Published)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["courseId"] = ["This course is not available for enrollment."]
            });

        // Check prerequisites
        var prerequisites = await _context.CoursePrerequisites
            .Where(p => p.CourseId == request.CourseId)
            .Include(p => p.PrerequisiteCourse)
            .ToListAsync(cancellationToken);

        foreach (var prereq in prerequisites)
        {
            var totalLessons = await _context.Lessons
                .CountAsync(l =>
                    l.Chapter.CourseId == prereq.PrerequisiteCourseId &&
                    l.Type != LessonType.Section, cancellationToken);

            if (totalLessons == 0) continue;

            var completedLessons = await _context.UserProgress
                .CountAsync(p =>
                    p.UserId == _currentUser.Id &&
                    p.Status == ProgressStatus.Completed &&
                    _context.Lessons.Any(l =>
                        l.Id == p.LessonId &&
                        l.Type != LessonType.Section &&
                        l.Chapter.CourseId == prereq.PrerequisiteCourseId), cancellationToken);

            if (completedLessons < totalLessons)
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    ["prerequisites"] = [$"You must complete \"{prereq.PrerequisiteCourse.Title}\" before enrolling in this course."]
                });
        }

        // Prevent duplicate active enrollments
        var existingActive = await _context.Enrollments
            .AnyAsync(e =>
                e.UserId == _currentUser.Id &&
                e.CourseId == request.CourseId &&
                e.Status == EnrollmentStatus.Active, cancellationToken);

        if (existingActive)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["courseId"] = ["You are already enrolled in this course."]
            });

        // Free courses → Active immediately; paid/membership → Pending
        var status = course.AccessType == AccessType.Free
            ? EnrollmentStatus.Active
            : EnrollmentStatus.Pending;

        var enrollment = new Enrollment(_currentUser.Id, course.Id, status);

        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new EnrollmentDto(
            enrollment.Id, enrollment.UserId, course.Id,
            course.Title, course.Image, enrollment.Status,
            enrollment.EnrolledAt, enrollment.ExpiresAt);
    }
}
