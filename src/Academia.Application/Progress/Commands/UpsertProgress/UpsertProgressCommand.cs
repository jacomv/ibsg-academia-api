using Academia.Application.Progress.Dtos;
using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Progress.Commands.UpsertProgress;

public record UpsertProgressCommand(
    Guid LessonId,
    ProgressStatus Status,
    int? VideoPosition,
    int? AudioPosition,
    decimal ProgressPercentage
) : IRequest<UpsertProgressResult>;
