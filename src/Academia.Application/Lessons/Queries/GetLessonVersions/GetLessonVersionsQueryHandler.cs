using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Lessons.Queries.GetLessonVersions;

public class GetLessonVersionsQueryHandler : IRequestHandler<GetLessonVersionsQuery, List<LessonVersionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLessonVersionsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<LessonVersionDto>> Handle(
        GetLessonVersionsQuery request, CancellationToken cancellationToken)
    {
        var lessonExists = await _context.Lessons
            .AnyAsync(l => l.Id == request.LessonId, cancellationToken);

        if (!lessonExists)
            throw new NotFoundException("Lesson", request.LessonId);

        return await _context.LessonVersions
            .Where(v => v.LessonId == request.LessonId)
            .Include(v => v.Author)
            .OrderByDescending(v => v.VersionNumber)
            .Select(v => new LessonVersionDto(
                v.Id,
                v.VersionNumber,
                v.Reason,
                v.AuthorId,
                v.Author.FirstName + " " + v.Author.LastName,
                v.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
