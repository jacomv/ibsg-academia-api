using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Users.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(Guid UserId, UserRole NewRole) : IRequest;
