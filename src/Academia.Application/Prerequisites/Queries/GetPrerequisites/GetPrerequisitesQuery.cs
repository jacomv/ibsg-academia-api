using MediatR;

namespace Academia.Application.Prerequisites.Queries.GetPrerequisites;

public record GetPrerequisitesQuery(Guid CourseId) : IRequest<List<PrerequisiteDto>>;

public record PrerequisiteDto(
    Guid CourseId,
    string CourseTitle,
    string? CourseImage,
    bool IsCompleted
);
