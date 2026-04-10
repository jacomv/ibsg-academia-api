using Academia.Application.Notes.Dtos;
using MediatR;

namespace Academia.Application.Notes.Queries.GetLessonNotes;

public record GetLessonNotesQuery(Guid LessonId) : IRequest<List<NoteDto>>;
