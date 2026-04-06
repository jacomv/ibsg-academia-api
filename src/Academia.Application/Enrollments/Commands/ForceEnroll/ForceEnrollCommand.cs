using Academia.Application.Enrollments.Dtos;
using MediatR;

namespace Academia.Application.Enrollments.Commands.ForceEnroll;

public record ForceEnrollCommand(
    Guid UserId,
    Guid CourseId,
    DateTime? ExpiresAt,
    string? Notes
) : IRequest<EnrollmentDto>;
