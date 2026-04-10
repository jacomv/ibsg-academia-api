using Academia.Application.Bookmarks.Dtos;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Bookmarks.Queries.GetMyBookmarks;

public class GetMyBookmarksQueryHandler : IRequestHandler<GetMyBookmarksQuery, List<BookmarkDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMyBookmarksQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<BookmarkDto>> Handle(
        GetMyBookmarksQuery request, CancellationToken cancellationToken)
    {
        return await _context.Bookmarks
            .Where(b => b.UserId == _currentUser.Id)
            .Include(b => b.Lesson)
                .ThenInclude(l => l.Chapter)
                    .ThenInclude(ch => ch.Course)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BookmarkDto(
                b.Id, b.LessonId, b.Lesson.Title,
                b.Lesson.Type,
                b.Lesson.Chapter.Course.Title,
                b.Lesson.Chapter.Title,
                b.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
