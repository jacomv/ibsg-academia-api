using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Lessons.Commands.UpdateLesson;

public record UpdateLessonCommand(
    Guid Id,
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
) : IRequest;
