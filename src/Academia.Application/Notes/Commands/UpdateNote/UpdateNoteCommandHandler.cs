using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Notes.Dtos;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Notes.Commands.UpdateNote;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, NoteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateNoteCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<NoteDto> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.StudentNotes
            .Include(n => n.Lesson)
                .ThenInclude(l => l.Chapter)
                    .ThenInclude(ch => ch.Course)
            .FirstOrDefaultAsync(n => n.Id == request.NoteId && n.UserId == _currentUser.Id,
                cancellationToken);

        if (note is null)
            throw new NotFoundException("Note", request.NoteId);

        note.Update(request.Content, request.TimestampSeconds);
        await _context.SaveChangesAsync(cancellationToken);

        return new NoteDto(
            note.Id, note.LessonId, note.Lesson.Title,
            note.Lesson.Chapter.Course.Title,
            note.Content, note.TimestampSeconds,
            note.CreatedAt, note.UpdatedAt);
    }
}
