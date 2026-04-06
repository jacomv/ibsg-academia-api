using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Events;
using Academia.Application.Progress.Dtos;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Progress.Commands.UpsertProgress;

public class UpsertProgressCommandHandler : IRequestHandler<UpsertProgressCommand, UpsertProgressResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IPublisher _publisher;

    public UpsertProgressCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IPublisher publisher)
    {
        _context = context;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task<UpsertProgressResult> Handle(
        UpsertProgressCommand request, CancellationToken cancellationToken)
    {
        // Load lesson with chapter and course context
        var lesson = await _context.Lessons
            .Include(l => l.Chapter)
                .ThenInclude(ch => ch.Course)
            .FirstOrDefaultAsync(l => l.Id == request.LessonId, cancellationToken);

        if (lesson is null)
            throw new NotFoundException("Lesson", request.LessonId);

        var progress = await _context.UserProgress
            .FirstOrDefaultAsync(p =>
                p.UserId == _currentUser.Id &&
                p.LessonId == request.LessonId, cancellationToken);

        var wasAlreadyCompleted = progress?.Status == ProgressStatus.Completed;

        if (progress is null)
        {
            progress = new UserProgress(_currentUser.Id, request.LessonId);
            await _context.UserProgress.AddAsync(progress, cancellationToken);
        }

        var justCompleted = false;

        if (request.Status == ProgressStatus.Completed && !wasAlreadyCompleted)
        {
            progress.MarkCompleted();
            justCompleted = true;
        }
        else if (!wasAlreadyCompleted)
        {
            progress.UpdatePosition(
                request.VideoPosition,
                request.AudioPosition,
                request.ProgressPercentage);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Check if the entire course is now complete
        if (justCompleted)
        {
            await CheckCourseCompletionAsync(
                lesson.Chapter.CourseId,
                lesson.Chapter.Course.Title,
                cancellationToken);
        }

        return new UpsertProgressResult(
            progress.LessonId,
            progress.Status,
            progress.ProgressPercentage,
            justCompleted);
    }

    private async Task CheckCourseCompletionAsync(
        Guid courseId, string courseTitle, CancellationToken ct)
    {
        // Get all lesson IDs in this course
        var totalLessons = await _context.Lessons
            .CountAsync(l => l.Chapter.CourseId == courseId, ct);

        if (totalLessons == 0) return;

        // Count how many the current user has completed
        var completedLessons = await _context.UserProgress
            .CountAsync(p =>
                p.UserId == _currentUser.Id &&
                p.Status == ProgressStatus.Completed &&
                _context.Lessons.Any(l =>
                    l.Id == p.LessonId &&
                    l.Chapter.CourseId == courseId), ct);

        if (completedLessons >= totalLessons)
        {
            await _publisher.Publish(
                new CourseCompletedEvent(_currentUser.Id, courseId, courseTitle), ct);
        }
    }
}
