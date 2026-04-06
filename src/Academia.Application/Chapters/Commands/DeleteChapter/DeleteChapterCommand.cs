using MediatR;

namespace Academia.Application.Chapters.Commands.DeleteChapter;

public record DeleteChapterCommand(Guid Id) : IRequest;
