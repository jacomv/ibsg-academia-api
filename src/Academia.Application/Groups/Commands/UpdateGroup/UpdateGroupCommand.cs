using MediatR;

namespace Academia.Application.Groups.Commands.UpdateGroup;

public record UpdateGroupCommand(Guid Id, string Name, string? Description) : IRequest;
