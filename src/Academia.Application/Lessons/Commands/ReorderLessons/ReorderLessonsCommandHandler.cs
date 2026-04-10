using System.Text.Json;
using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Lessons.Commands.ReorderLessons;

public class ReorderLessonsCommandHandler : IRequestHandler<ReorderLessonsCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public ReorderLessonsCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ReorderLessonsCommand request, CancellationToken cancellationToken)
    {
        var lessons = await _context.Lessons
            .Where(l => l.ChapterId == request.ChapterId)
            .OrderBy(l => l.Order)
            .ToListAsync(cancellationToken);

        if (lessons.Count == 0)
            throw new NotFoundException("Chapter lessons", request.ChapterId);

        var lessonIds = lessons.Select(l => l.Id).ToHashSet();
        if (!request.OrderedLessonIds.All(id => lessonIds.Contains(id))
            || request.OrderedLessonIds.Count != lessons.Count)
            throw new ValidationException(new[] { "Ordered IDs must match existing lessons exactly." });

        var previousOrder = lessons.Select(l => new { l.Id, l.Order }).ToList();

        for (var i = 0; i < request.OrderedLessonIds.Count; i++)
        {
            var lesson = lessons.First(l => l.Id == request.OrderedLessonIds[i]);
            lesson.Update(
                lesson.Title, lesson.Type, lesson.TextContent, lesson.VideoUrl,
                lesson.AudioUrl, lesson.PdfFile, lesson.DurationMinutes,
                i + 1, lesson.RequiresCompletingPrevious, lesson.AvailableFrom);
        }

        var audit = new ReorderAudit(
            "Lesson", request.ChapterId, _currentUser.Id,
            JsonSerializer.Serialize(previousOrder),
            JsonSerializer.Serialize(request.OrderedLessonIds));
        await _context.ReorderAudits.AddAsync(audit, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
