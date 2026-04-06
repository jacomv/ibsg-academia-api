using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Chapters.Commands.CreateChapter;

public class CreateChapterCommandHandler : IRequestHandler<CreateChapterCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateChapterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateChapterCommand request, CancellationToken cancellationToken)
    {
        var courseExists = await _context.Courses
            .AnyAsync(c => c.Id == request.CourseId, cancellationToken);

        if (!courseExists)
            throw new NotFoundException("Course", request.CourseId);

        var chapter = new Chapter(
            request.CourseId, request.Title,
            request.Description, request.Order, request.AvailableFrom);

        await _context.Chapters.AddAsync(chapter, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return chapter.Id;
    }
}
