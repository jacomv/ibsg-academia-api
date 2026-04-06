using Academia.Application.Auth.Commands.Login;
using MediatR;

namespace Academia.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string Token) : IRequest<LoginResult>;
