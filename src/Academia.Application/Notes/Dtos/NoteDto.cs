namespace Academia.Application.Notes.Dtos;

public record NoteDto(
    Guid Id,
    Guid LessonId,
    string LessonTitle,
    string? CourseName,
    string Content,
    int? TimestampSeconds,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
