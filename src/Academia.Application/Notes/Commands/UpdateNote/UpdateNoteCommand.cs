using Academia.Application.Notes.Dtos;
using MediatR;

namespace Academia.Application.Notes.Commands.UpdateNote;

public record UpdateNoteCommand(Guid NoteId, string Content, int? TimestampSeconds) : IRequest<NoteDto>;
