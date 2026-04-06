using Academia.Application.Admin.Dtos;
using MediatR;

namespace Academia.Application.Admin.Queries.GetDashboard;

public record GetAdminDashboardQuery : IRequest<AdminDashboardDto>;
