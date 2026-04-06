using MediatR;

namespace Academia.Application.Groups.Commands.DeleteGroup;

public record DeleteGroupCommand(Guid Id) : IRequest;
