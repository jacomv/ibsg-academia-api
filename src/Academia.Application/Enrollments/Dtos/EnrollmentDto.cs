using Academia.Domain.Enums;

namespace Academia.Application.Enrollments.Dtos;

public record EnrollmentDto(
    Guid Id,
    Guid UserId,
    Guid CourseId,
    string CourseTitle,
    string? CourseImage,
    EnrollmentStatus Status,
    DateTime EnrolledAt,
    DateTime? ExpiresAt
);

public record EnrollmentDetailDto(
    Guid Id,
    Guid UserId,
    string UserFullName,
    string UserEmail,
    Guid CourseId,
    string CourseTitle,
    EnrollmentStatus Status,
    DateTime EnrolledAt,
    DateTime? ExpiresAt,
    string? Notes
);
