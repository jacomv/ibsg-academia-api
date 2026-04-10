using Academia.Application.Notes.Dtos;
using MediatR;

namespace Academia.Application.Notes.Queries.GetMyNotes;

public record GetMyNotesQuery() : IRequest<List<NoteDto>>;
