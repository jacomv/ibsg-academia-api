using System.Text.Json;
using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Chapters.Commands.ReorderChapters;

public class ReorderChaptersCommandHandler : IRequestHandler<ReorderChaptersCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public ReorderChaptersCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ReorderChaptersCommand request, CancellationToken cancellationToken)
    {
        var chapters = await _context.Chapters
            .Where(c => c.CourseId == request.CourseId)
            .OrderBy(c => c.Order)
            .ToListAsync(cancellationToken);

        if (chapters.Count == 0)
            throw new NotFoundException("Course chapters", request.CourseId);

        var chapterIds = chapters.Select(c => c.Id).ToHashSet();
        if (!request.OrderedChapterIds.All(id => chapterIds.Contains(id))
            || request.OrderedChapterIds.Count != chapters.Count)
            throw new ValidationException(new[] { "Ordered IDs must match existing chapters exactly." });

        var previousOrder = chapters.Select(c => new { c.Id, c.Order }).ToList();

        for (var i = 0; i < request.OrderedChapterIds.Count; i++)
        {
            var chapter = chapters.First(c => c.Id == request.OrderedChapterIds[i]);
            chapter.Update(chapter.Title, chapter.Description, i + 1, chapter.AvailableFrom);
        }

        var audit = new ReorderAudit(
            "Chapter", request.CourseId, _currentUser.Id,
            JsonSerializer.Serialize(previousOrder),
            JsonSerializer.Serialize(request.OrderedChapterIds));
        await _context.ReorderAudits.AddAsync(audit, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
