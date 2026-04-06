using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Chapters.Commands.UpdateChapter;

public class UpdateChapterCommandHandler : IRequestHandler<UpdateChapterCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateChapterCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateChapterCommand request, CancellationToken cancellationToken)
    {
        var chapter = await _context.Chapters
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (chapter is null)
            throw new NotFoundException("Chapter", request.Id);

        chapter.Update(request.Title, request.Description, request.Order, request.AvailableFrom);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
