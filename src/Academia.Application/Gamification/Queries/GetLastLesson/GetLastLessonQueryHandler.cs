using Academia.Application.Common.Interfaces;
using Academia.Application.Gamification.Dtos;
using Academia.Domain.Enums;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Gamification.Queries.GetLastLesson;

public class GetLastLessonQueryHandler : IRequestHandler<GetLastLessonQuery, LastLessonDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetLastLessonQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<LastLessonDto?> Handle(GetLastLessonQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id;

        var progress = await _context.UserProgress
            .Include(p => p.Lesson)
                .ThenInclude(l => l.Chapter)
                    .ThenInclude(ch => ch.Course)
            .Where(p => p.UserId == userId && p.Status != ProgressStatus.Completed)
            .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (progress is null)
            return null;

        var lesson = progress.Lesson;
        var course = lesson.Chapter.Course;

        return new LastLessonDto(
            lesson.Id,
            lesson.Title,
            course.Id,
            course.Title,
            course.Image,
            progress.ProgressPercentage,
            lesson.Type.ToString());
    }
}
