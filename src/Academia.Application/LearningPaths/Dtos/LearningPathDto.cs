using Academia.Domain.Enums;

namespace Academia.Application.LearningPaths.Dtos;

public record LearningPathListDto(
    Guid Id,
    string Name,
    string? Description,
    string? Image,
    CourseStatus Status,
    AccessType AccessType,
    decimal? Price,
    int GlobalOrder,
    int CourseCount,
    DateTime CreatedAt
);

public record LearningPathDetailDto(
    Guid Id,
    string Name,
    string? Description,
    string? Image,
    CourseStatus Status,
    AccessType AccessType,
    decimal? Price,
    int GlobalOrder,
    List<PathCourseDto> Courses,
    DateTime CreatedAt
);

public record PathCourseDto(
    Guid LearningPathCourseId,
    Guid CourseId,
    string Title,
    string? Image,
    CourseStatus Status,
    int Order,
    bool IsRequired,
    int EstimatedDuration
);
