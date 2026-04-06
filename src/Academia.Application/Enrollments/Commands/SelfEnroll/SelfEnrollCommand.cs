using Academia.Application.Enrollments.Dtos;
using MediatR;

namespace Academia.Application.Enrollments.Commands.SelfEnroll;

public record SelfEnrollCommand(Guid CourseId) : IRequest<EnrollmentDto>;
