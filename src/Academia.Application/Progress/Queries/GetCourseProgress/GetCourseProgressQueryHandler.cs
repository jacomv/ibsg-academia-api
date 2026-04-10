using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Progress.Dtos;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Progress.Queries.GetCourseProgress;

public class GetCourseProgressQueryHandler : IRequestHandler<GetCourseProgressQuery, CourseProgressDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetCourseProgressQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CourseProgressDto> Handle(
        GetCourseProgressQuery request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Chapters.OrderBy(ch => ch.Order))
                .ThenInclude(ch => ch.Lessons.OrderBy(l => l.Order))
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.CourseId);

        var lessonIds = course.Chapters
            .SelectMany(ch => ch.Lessons.Where(l => l.Type != LessonType.Section).Select(l => l.Id))
            .ToList();

        // Load all progress records for this user+course in one query
        var progressMap = await _context.UserProgress
            .Where(p => p.UserId == _currentUser.Id && lessonIds.Contains(p.LessonId))
            .AsNoTracking()
            .ToDictionaryAsync(p => p.LessonId, cancellationToken);

        var totalLessons = lessonIds.Count;
        var completedLessons = progressMap.Values
            .Count(p => p.Status == ProgressStatus.Completed);

        var overallPercentage = totalLessons > 0
            ? Math.Round((decimal)completedLessons / totalLessons * 100, 1)
            : 0m;

        var chapterDtos = course.Chapters.Select(ch =>
        {
            var chLessons = ch.Lessons.Select(l =>
            {
                progressMap.TryGetValue(l.Id, out var prog);
                return new LessonProgressDto(
                    l.Id, l.Title, l.Type, l.Order,
                    prog?.Status ?? ProgressStatus.NotStarted,
                    prog?.ProgressPercentage ?? 0,
                    prog?.VideoPosition,
                    prog?.AudioPosition,
                    prog?.CompletedAt);
            }).ToList();

            var playableLessons = ch.Lessons.Count(l => l.Type != LessonType.Section);
            return new ChapterProgressDto(
                ch.Id, ch.Title, ch.Order,
                playableLessons,
                chLessons.Count(l => l.Status == ProgressStatus.Completed),
                chLessons);
        }).ToList();

        return new CourseProgressDto(
            course.Id, course.Title,
            totalLessons, completedLessons,
            overallPercentage, chapterDtos);
    }
}
