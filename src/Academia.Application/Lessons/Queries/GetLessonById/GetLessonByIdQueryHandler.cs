using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Courses.Dtos;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Lessons.Queries.GetLessonById;

public class GetLessonByIdQueryHandler : IRequestHandler<GetLessonByIdQuery, LessonContentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetLessonByIdQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<LessonContentDto> Handle(
        GetLessonByIdQuery request, CancellationToken cancellationToken)
    {
        // Load lesson with full context needed for access checks
        var lesson = await _context.Lessons
            .Include(l => l.Chapter)
                .ThenInclude(ch => ch.Course)
            .Include(l => l.Chapter)
                .ThenInclude(ch => ch.Lessons.OrderBy(x => x.Order))
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.LessonId, cancellationToken);

        if (lesson is null)
            throw new NotFoundException("Lesson", request.LessonId);

        var (isLocked, lockReason) = await EvaluateAccessAsync(lesson, cancellationToken);

        // Get sibling lessons for prev/next navigation (skip sections)
        var playable = lesson.Chapter.Lessons
            .Where(l => l.Type != LessonType.Section)
            .OrderBy(l => l.Order).ToList();
        var currentIndex = playable.FindIndex(l => l.Id == lesson.Id);
        var previousId = currentIndex > 0 ? playable[currentIndex - 1].Id : (Guid?)null;
        var nextId = currentIndex < playable.Count - 1 ? playable[currentIndex + 1].Id : (Guid?)null;

        // Load attachments
        var attachments = await _context.LessonAttachments
            .Where(a => a.LessonId == lesson.Id)
            .OrderBy(a => a.Order)
            .Select(a => new AttachmentDto(a.Id, a.FileName, a.FileUrl, a.FileType, a.FileSize, a.Order))
            .ToListAsync(cancellationToken);

        // Check bookmark status
        var isBookmarked = await _context.Bookmarks
            .AnyAsync(b => b.UserId == _currentUser.Id && b.LessonId == lesson.Id, cancellationToken);

        return new LessonContentDto(
            Id: lesson.Id,
            Title: lesson.Title,
            Type: lesson.Type,
            TextContent: isLocked ? null : lesson.TextContent,
            VideoUrl: isLocked ? null : lesson.VideoUrl,
            AudioUrl: isLocked ? null : lesson.AudioUrl,
            PdfFile: isLocked ? null : lesson.PdfFile,
            DurationMinutes: lesson.DurationMinutes,
            RequiresCompletingPrevious: lesson.RequiresCompletingPrevious,
            IsLocked: isLocked,
            LockReason: lockReason,
            PreviousLessonId: previousId,
            NextLessonId: nextId,
            Attachments: attachments,
            IsBookmarked: isBookmarked
        );
    }

    private async Task<(bool IsLocked, string? Reason)> EvaluateAccessAsync(
        Domain.Entities.Lesson lesson, CancellationToken ct)
    {
        var course = lesson.Chapter.Course;

        // Admins and teachers always have access
        if (_currentUser.IsAdmin || _currentUser.IsTeacher)
            return (false, null);

        // Enrollment check for non-free courses
        if (course.AccessType != AccessType.Free)
        {
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.UserId == _currentUser.Id &&
                    e.CourseId == course.Id &&
                    e.Status == EnrollmentStatus.Active, ct);

            if (!isEnrolled)
                return (true, "You must be enrolled in this course to access this lesson.");
        }

        // Chapter release date check
        if (lesson.Chapter.IsLocked)
            return (true, $"This chapter will be available on {lesson.Chapter.AvailableFrom:MMM dd, yyyy}.");

        // Lesson release date check
        if (lesson.IsLockedByDate)
            return (true, $"This lesson will be available on {lesson.AvailableFrom:MMM dd, yyyy}.");

        // Sequential lock check (skip sections)
        if (lesson.RequiresCompletingPrevious)
        {
            var siblings = lesson.Chapter.Lessons
                .Where(l => l.Type != LessonType.Section)
                .OrderBy(l => l.Order).ToList();
            var index = siblings.FindIndex(l => l.Id == lesson.Id);

            if (index > 0)
            {
                var previousLesson = siblings[index - 1];
                var previousCompleted = await _context.UserProgress
                    .AnyAsync(p =>
                        p.UserId == _currentUser.Id &&
                        p.LessonId == previousLesson.Id &&
                        p.Status == ProgressStatus.Completed, ct);

                if (!previousCompleted)
                    return (true, "You must complete the previous lesson first.");
            }
        }

        return (false, null);
    }
}
