using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Notes.Dtos;
using Academia.Domain.Entities;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Notes.Commands.CreateNote;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreateNoteCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Chapter)
                .ThenInclude(ch => ch.Course)
            .FirstOrDefaultAsync(l => l.Id == request.LessonId, cancellationToken);

        if (lesson is null)
            throw new NotFoundException("Lesson", request.LessonId);

        var note = new StudentNote(_currentUser.Id, request.LessonId, request.Content, request.TimestampSeconds);
        await _context.StudentNotes.AddAsync(note, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new NoteDto(
            note.Id, note.LessonId, lesson.Title,
            lesson.Chapter.Course.Title,
            note.Content, note.TimestampSeconds,
            note.CreatedAt, note.UpdatedAt);
    }
}
