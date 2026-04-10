using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Events;
using Academia.Application.Progress.Dtos;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Progress.Commands.CompleteLesson;

public class CompleteLessonCommandHandler : IRequestHandler<CompleteLessonCommand, UpsertProgressResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IPublisher _publisher;

    public CompleteLessonCommandHandler(
        IApplicationDbContext context, ICurrentUser currentUser, IPublisher publisher)
    {
        _context = context;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task<UpsertProgressResult> Handle(
        CompleteLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Chapter)
                .ThenInclude(ch => ch.Course)
            .FirstOrDefaultAsync(l => l.Id == request.LessonId, cancellationToken);

        if (lesson is null)
            throw new NotFoundException("Lesson", request.LessonId);

        // Only non-video, non-section lessons can be manually completed
        if (lesson.Type == LessonType.Video)
            throw new ValidationException(new[] { "Video lessons must be completed by finishing playback." });

        if (lesson.Type == LessonType.Section)
            throw new ValidationException(new[] { "Sections cannot be marked as completed." });

        var progress = await _context.UserProgress
            .FirstOrDefaultAsync(p =>
                p.UserId == _currentUser.Id && p.LessonId == request.LessonId,
                cancellationToken);

        if (progress?.Status == ProgressStatus.Completed)
            return new UpsertProgressResult(request.LessonId, ProgressStatus.Completed, 100, false);

        if (progress is null)
        {
            progress = new UserProgress(_currentUser.Id, request.LessonId);
            await _context.UserProgress.AddAsync(progress, cancellationToken);
        }

        progress.MarkCompleted();
        await _context.SaveChangesAsync(cancellationToken);

        // Award points + streak
        var pts = new PointTransaction(
            _currentUser.Id, 10, $"Lesson completed: {lesson.Title}", lesson.Id.ToString());
        await _context.PointTransactions.AddAsync(pts, cancellationToken);

        var streak = await _context.UserStreaks
            .FirstOrDefaultAsync(s => s.UserId == _currentUser.Id, cancellationToken);
        if (streak is null)
        {
            streak = new UserStreak(_currentUser.Id);
            await _context.UserStreaks.AddAsync(streak, cancellationToken);
        }
        streak.RecordActivity();
        await _context.SaveChangesAsync(cancellationToken);

        // Check course completion
        var courseId = lesson.Chapter.CourseId;
        var totalLessons = await _context.Lessons
            .CountAsync(l => l.Chapter.CourseId == courseId && l.Type != LessonType.Section,
                cancellationToken);

        var completedLessons = await _context.UserProgress
            .CountAsync(p =>
                p.UserId == _currentUser.Id &&
                p.Status == ProgressStatus.Completed &&
                _context.Lessons.Any(l =>
                    l.Id == p.LessonId &&
                    l.Type != LessonType.Section &&
                    l.Chapter.CourseId == courseId), cancellationToken);

        if (totalLessons > 0 && completedLessons >= totalLessons)
        {
            await _publisher.Publish(
                new CourseCompletedEvent(_currentUser.Id, courseId, lesson.Chapter.Course.Title),
                cancellationToken);
        }

        return new UpsertProgressResult(request.LessonId, ProgressStatus.Completed, 100, true);
    }
}
