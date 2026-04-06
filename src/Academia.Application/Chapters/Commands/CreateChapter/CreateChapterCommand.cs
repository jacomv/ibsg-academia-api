using MediatR;

namespace Academia.Application.Chapters.Commands.CreateChapter;

public record CreateChapterCommand(
    Guid CourseId,
    string Title,
    string? Description,
    int Order,
    DateTime? AvailableFrom
) : IRequest<Guid>;
