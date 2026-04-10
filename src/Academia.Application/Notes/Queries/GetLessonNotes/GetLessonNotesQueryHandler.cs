using Academia.Application.Common.Interfaces;
using Academia.Application.Notes.Dtos;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Notes.Queries.GetLessonNotes;

public class GetLessonNotesQueryHandler : IRequestHandler<GetLessonNotesQuery, List<NoteDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetLessonNotesQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<NoteDto>> Handle(
        GetLessonNotesQuery request, CancellationToken cancellationToken)
    {
        return await _context.StudentNotes
            .Where(n => n.UserId == _currentUser.Id && n.LessonId == request.LessonId)
            .Include(n => n.Lesson)
                .ThenInclude(l => l.Chapter)
                    .ThenInclude(ch => ch.Course)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NoteDto(
                n.Id, n.LessonId, n.Lesson.Title,
                n.Lesson.Chapter.Course.Title,
                n.Content, n.TimestampSeconds,
                n.CreatedAt, n.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
