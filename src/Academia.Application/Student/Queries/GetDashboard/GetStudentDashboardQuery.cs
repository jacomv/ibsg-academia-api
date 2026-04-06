using Academia.Application.Student.Dtos;
using MediatR;

namespace Academia.Application.Student.Queries.GetDashboard;

public record GetStudentDashboardQuery : IRequest<StudentDashboardDto>;
