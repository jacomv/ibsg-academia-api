using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role
) : IRequest<Guid>;
