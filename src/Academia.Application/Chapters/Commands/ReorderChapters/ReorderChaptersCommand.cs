using MediatR;

namespace Academia.Application.Chapters.Commands.ReorderChapters;

public record ReorderChaptersCommand(
    Guid CourseId,
    List<Guid> OrderedChapterIds
) : IRequest;
