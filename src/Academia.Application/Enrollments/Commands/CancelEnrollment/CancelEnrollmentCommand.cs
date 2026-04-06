using MediatR;

namespace Academia.Application.Enrollments.Commands.CancelEnrollment;

public record CancelEnrollmentCommand(Guid EnrollmentId) : IRequest;
