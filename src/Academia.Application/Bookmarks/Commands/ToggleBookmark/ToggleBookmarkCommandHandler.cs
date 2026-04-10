using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Bookmarks.Commands.ToggleBookmark;

public class ToggleBookmarkCommandHandler : IRequestHandler<ToggleBookmarkCommand, ToggleBookmarkResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public ToggleBookmarkCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ToggleBookmarkResult> Handle(
        ToggleBookmarkCommand request, CancellationToken cancellationToken)
    {
        var lessonExists = await _context.Lessons
            .AnyAsync(l => l.Id == request.LessonId, cancellationToken);

        if (!lessonExists)
            throw new NotFoundException("Lesson", request.LessonId);

        var existing = await _context.Bookmarks
            .FirstOrDefaultAsync(b =>
                b.UserId == _currentUser.Id && b.LessonId == request.LessonId,
                cancellationToken);

        if (existing is not null)
        {
            _context.Bookmarks.Remove(existing);
            await _context.SaveChangesAsync(cancellationToken);
            return new ToggleBookmarkResult(false);
        }

        var bookmark = new Bookmark(_currentUser.Id, request.LessonId);
        await _context.Bookmarks.AddAsync(bookmark, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new ToggleBookmarkResult(true);
    }
}
