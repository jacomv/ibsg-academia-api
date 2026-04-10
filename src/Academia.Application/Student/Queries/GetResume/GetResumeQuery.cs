using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Student.Queries.GetResume;

public record GetResumeQuery() : IRequest<ResumeDto?>;

public record ResumeDto(
    Guid CourseId,
    string CourseTitle,
    string? CourseImage,
    decimal CourseProgressPercentage,
    Guid LessonId,
    string LessonTitle,
    LessonType LessonType,
    int? VideoPosition,
    int? AudioPosition,
    Guid ChapterId,
    string ChapterTitle
);
