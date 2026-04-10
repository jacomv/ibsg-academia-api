using Academia.Application.Bookmarks.Dtos;
using MediatR;

namespace Academia.Application.Bookmarks.Queries.GetMyBookmarks;

public record GetMyBookmarksQuery() : IRequest<List<BookmarkDto>>;
