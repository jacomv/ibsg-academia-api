using Academia.Application.Enrollments.Dtos;
using MediatR;

namespace Academia.Application.Enrollments.Queries.GetMyEnrollments;

public record GetMyEnrollmentsQuery : IRequest<List<EnrollmentDto>>;
