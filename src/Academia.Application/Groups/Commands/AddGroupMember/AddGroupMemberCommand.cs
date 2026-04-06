using MediatR;

namespace Academia.Application.Groups.Commands.AddGroupMember;

public record AddGroupMemberCommand(Guid GroupId, Guid UserId) : IRequest;
