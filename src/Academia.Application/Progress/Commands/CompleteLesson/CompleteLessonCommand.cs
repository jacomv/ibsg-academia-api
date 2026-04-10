using Academia.Application.Progress.Dtos;
using MediatR;

namespace Academia.Application.Progress.Commands.CompleteLesson;

public record CompleteLessonCommand(Guid LessonId) : IRequest<UpsertProgressResult>;
