using Academia.Application.Courses.Dtos;
using MediatR;

namespace Academia.Application.Lessons.Queries.GetAdminLessonById;

public record GetAdminLessonByIdQuery(Guid LessonId) : IRequest<AdminLessonDetailDto>;

public record AdminLessonDetailDto(
    Guid Id,
    Guid ChapterId,
    string ChapterTitle,
    Guid CourseId,
    string CourseTitle,
    string Title,
    Domain.Enums.LessonType Type,
    string? TextContent,
    string? VideoUrl,
    string? AudioUrl,
    string? PdfFile,
    int? DurationMinutes,
    int Order,
    bool RequiresCompletingPrevious,
    DateTime? AvailableFrom,
    List<AttachmentDto> Attachments,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
