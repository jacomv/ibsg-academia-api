using Academia.Domain.Enums;

namespace Academia.Application.Courses.Dtos;

public record CourseListDto(
    Guid Id,
    string Title,
    string? Description,
    string? Image,
    CourseStatus Status,
    AccessType AccessType,
    decimal? Price,
    int? EstimatedDuration,
    TeacherSummaryDto? Teacher,
    int ChapterCount,
    int LessonCount,
    DateTime CreatedAt
);

public record CourseDetailDto(
    Guid Id,
    string Title,
    string? Description,
    string? Image,
    CourseStatus Status,
    AccessType AccessType,
    decimal? Price,
    int? EstimatedDuration,
    TeacherSummaryDto? Teacher,
    List<ChapterDto> Chapters,
    DateTime CreatedAt
);

public record ChapterDto(
    Guid Id,
    string Title,
    string? Description,
    int Order,
    DateTime? AvailableFrom,
    bool IsLocked,
    List<LessonSummaryDto> Lessons,
    ExamSummaryDto? Exam,
    int? EstimatedDurationMinutes
);

public record LessonSummaryDto(
    Guid Id,
    string Title,
    LessonType Type,
    int Order,
    int? DurationMinutes,
    bool RequiresCompletingPrevious,
    bool IsLockedByDate
);

public record LessonContentDto(
    Guid Id,
    string Title,
    LessonType Type,
    string? TextContent,
    string? VideoUrl,
    string? AudioUrl,
    string? PdfFile,
    int? DurationMinutes,
    bool RequiresCompletingPrevious,
    bool IsLocked,
    string? LockReason,
    Guid? PreviousLessonId,
    Guid? NextLessonId,
    List<AttachmentDto>? Attachments,
    bool IsBookmarked
);

public record AttachmentDto(
    Guid Id,
    string FileName,
    string FileUrl,
    string FileType,
    long FileSize,
    int Order
);

public record TeacherSummaryDto(Guid Id, string FullName, string? Avatar);

public record ExamSummaryDto(Guid Id, string Title, decimal PassingScore, int MaxAttempts);
