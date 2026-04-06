using MediatR;

namespace Academia.Application.Users.Commands.UpdateOwnProfile;

public record UpdateOwnProfileCommand(
    string FirstName,
    string LastName,
    string? Avatar
) : IRequest;
