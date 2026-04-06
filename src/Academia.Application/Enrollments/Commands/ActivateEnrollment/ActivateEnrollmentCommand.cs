using MediatR;

namespace Academia.Application.Enrollments.Commands.ActivateEnrollment;

public record ActivateEnrollmentCommand(Guid EnrollmentId) : IRequest;
