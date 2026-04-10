using MediatR;

namespace Academia.Application.Courses.Queries.GetCourseVersions;

public record GetCourseVersionsQuery(Guid CourseId) : IRequest<List<CourseVersionDto>>;

public record CourseVersionDto(
    Guid Id,
    int VersionNumber,
    string Reason,
    Guid AuthorId,
    string AuthorName,
    DateTime CreatedAt
);
