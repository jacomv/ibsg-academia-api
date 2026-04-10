using Academia.Application.Common.Interfaces;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Student.Queries.GetResume;

public class GetResumeQueryHandler : IRequestHandler<GetResumeQuery, ResumeDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetResumeQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ResumeDto?> Handle(
        GetResumeQuery request, CancellationToken cancellationToken)
    {
        // Find the most recently updated progress record that is NOT completed
        // (i.e., still in progress) — or the most recently completed one
        var lastProgress = await _context.UserProgress
            .Where(p => p.UserId == _currentUser.Id)
            .Include(p => p.Lesson)
                .ThenInclude(l => l.Chapter)
                    .ThenInclude(ch => ch.Course)
            .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastProgress is null)
            return null;

        var lesson = lastProgress.Lesson;
        var chapter = lesson.Chapter;
        var course = chapter.Course;

        // If last lesson was completed, find the next uncompleted lesson in the course
        if (lastProgress.Status == ProgressStatus.Completed)
        {
            var nextLesson = await FindNextUncompletedLessonAsync(course.Id, cancellationToken);
            if (nextLesson is not null)
            {
                lesson = nextLesson;
                // Re-fetch chapter context
                var ch = await _context.Chapters
                    .FirstAsync(c => c.Id == lesson.ChapterId, cancellationToken);

                var progress = await CalculateCourseProgressAsync(course.Id, cancellationToken);

                return new ResumeDto(
                    course.Id, course.Title, course.Image, progress,
                    lesson.Id, lesson.Title, lesson.Type,
                    null, null,
                    ch.Id, ch.Title);
            }
        }

        var courseProgress = await CalculateCourseProgressAsync(course.Id, cancellationToken);

        return new ResumeDto(
            course.Id, course.Title, course.Image, courseProgress,
            lesson.Id, lesson.Title, lesson.Type,
            lastProgress.VideoPosition, lastProgress.AudioPosition,
            chapter.Id, chapter.Title);
    }

    private async Task<Domain.Entities.Lesson?> FindNextUncompletedLessonAsync(
        Guid courseId, CancellationToken ct)
    {
        var allLessons = await _context.Lessons
            .Where(l => l.Chapter.CourseId == courseId && l.Type != LessonType.Section)
            .Include(l => l.Chapter)
            .OrderBy(l => l.Chapter.Order)
            .ThenBy(l => l.Order)
            .ToListAsync(ct);

        var completedIds = await _context.UserProgress
            .Where(p => p.UserId == _currentUser.Id && p.Status == ProgressStatus.Completed)
            .Select(p => p.LessonId)
            .ToListAsync(ct);

        var completedSet = completedIds.ToHashSet();

        return allLessons.FirstOrDefault(l => !completedSet.Contains(l.Id));
    }

    private async Task<decimal> CalculateCourseProgressAsync(Guid courseId, CancellationToken ct)
    {
        var total = await _context.Lessons
            .CountAsync(l => l.Chapter.CourseId == courseId && l.Type != LessonType.Section, ct);

        if (total == 0) return 0;

        var completed = await _context.UserProgress
            .CountAsync(p =>
                p.UserId == _currentUser.Id &&
                p.Status == ProgressStatus.Completed &&
                _context.Lessons.Any(l =>
                    l.Id == p.LessonId &&
                    l.Type != LessonType.Section &&
                    l.Chapter.CourseId == courseId), ct);

        return Math.Round((decimal)completed / total * 100, 1);
    }
}
