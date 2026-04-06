using MediatR;

namespace Academia.Application.Lessons.Commands.DeleteLesson;

public record DeleteLessonCommand(Guid Id) : IRequest;
