using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Chapters.Commands.DeleteChapter;

public class DeleteChapterCommandHandler : IRequestHandler<DeleteChapterCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteChapterCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteChapterCommand request, CancellationToken cancellationToken)
    {
        var chapter = await _context.Chapters
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (chapter is null)
            throw new NotFoundException("Chapter", request.Id);

        _context.Chapters.Remove(chapter);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
