using MediatR;

namespace Academia.Application.Groups.Commands.CreateGroup;

public record CreateGroupCommand(string Name, string? Description) : IRequest<Guid>;
