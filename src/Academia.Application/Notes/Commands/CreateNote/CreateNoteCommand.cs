using Academia.Application.Notes.Dtos;
using MediatR;

namespace Academia.Application.Notes.Commands.CreateNote;

public record CreateNoteCommand(
    Guid LessonId,
    string Content,
    int? TimestampSeconds
) : IRequest<NoteDto>;
