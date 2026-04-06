using Academia.Domain.Enums;

namespace Academia.Application.Progress.Dtos;

public record CourseProgressDto(
    Guid CourseId,
    string CourseTitle,
    int TotalLessons,
    int CompletedLessons,
    decimal ProgressPercentage,
    List<ChapterProgressDto> Chapters
);

public record ChapterProgressDto(
    Guid ChapterId,
    string Title,
    int Order,
    int TotalLessons,
    int CompletedLessons,
    List<LessonProgressDto> Lessons
);

public record LessonProgressDto(
    Guid LessonId,
    string Title,
    LessonType Type,
    int Order,
    ProgressStatus Status,
    decimal ProgressPercentage,
    int? VideoPosition,
    int? AudioPosition,
    DateTime? CompletedAt
);

public record UpsertProgressResult(
    Guid LessonId,
    ProgressStatus Status,
    decimal ProgressPercentage,
    bool JustCompleted
);
