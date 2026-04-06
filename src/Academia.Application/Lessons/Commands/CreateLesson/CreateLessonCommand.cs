using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Lessons.Commands.CreateLesson;

public record CreateLessonCommand(
    Guid ChapterId,
    string Title,
    LessonType Type,
    string? TextContent,
    string? VideoUrl,
    string? AudioUrl,
    string? PdfFile,
    int? DurationMinutes,
    int Order,
    bool RequiresCompletingPrevious,
    DateTime? AvailableFrom
) : IRequest<Guid>;
