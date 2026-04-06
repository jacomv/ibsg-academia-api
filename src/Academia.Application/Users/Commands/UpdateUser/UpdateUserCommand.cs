using MediatR;

namespace Academia.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id, string FirstName, string LastName, string? Avatar
) : IRequest;
