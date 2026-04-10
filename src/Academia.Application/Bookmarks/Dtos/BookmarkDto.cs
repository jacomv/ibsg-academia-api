using Academia.Domain.Enums;

namespace Academia.Application.Bookmarks.Dtos;

public record BookmarkDto(
    Guid Id,
    Guid LessonId,
    string LessonTitle,
    LessonType LessonType,
    string CourseName,
    string ChapterName,
    DateTime CreatedAt
);
