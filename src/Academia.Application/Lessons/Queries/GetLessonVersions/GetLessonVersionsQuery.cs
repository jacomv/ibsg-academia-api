using MediatR;

namespace Academia.Application.Lessons.Queries.GetLessonVersions;

public record GetLessonVersionsQuery(Guid LessonId) : IRequest<List<LessonVersionDto>>;

public record LessonVersionDto(
    Guid Id,
    int VersionNumber,
    string Reason,
    Guid AuthorId,
    string AuthorName,
    DateTime CreatedAt
);
