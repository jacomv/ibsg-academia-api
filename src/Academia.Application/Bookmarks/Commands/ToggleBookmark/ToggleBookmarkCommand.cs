using MediatR;

namespace Academia.Application.Bookmarks.Commands.ToggleBookmark;

public record ToggleBookmarkCommand(Guid LessonId) : IRequest<ToggleBookmarkResult>;

public record ToggleBookmarkResult(bool IsBookmarked);
