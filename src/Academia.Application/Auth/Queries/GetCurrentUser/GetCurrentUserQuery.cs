using Academia.Application.Auth.Commands.Login;
using MediatR;

namespace Academia.Application.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserInfo>;
