using MediatR;

namespace Academia.Application.Notes.Commands.DeleteNote;

public record DeleteNoteCommand(Guid NoteId) : IRequest;
