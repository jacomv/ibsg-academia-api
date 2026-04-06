using MediatR;

namespace Academia.Application.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;
