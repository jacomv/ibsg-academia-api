using MediatR;

namespace Academia.Application.Chapters.Commands.UpdateChapter;

public record UpdateChapterCommand(
    Guid Id,
    string Title,
    string? Description,
    int Order,
    DateTime? AvailableFrom
) : IRequest;
