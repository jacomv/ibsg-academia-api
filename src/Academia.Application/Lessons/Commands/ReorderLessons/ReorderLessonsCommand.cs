using MediatR;

namespace Academia.Application.Lessons.Commands.ReorderLessons;

public record ReorderLessonsCommand(
    Guid ChapterId,
    List<Guid> OrderedLessonIds
) : IRequest;
