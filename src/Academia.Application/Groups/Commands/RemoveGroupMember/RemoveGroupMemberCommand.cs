using MediatR;

namespace Academia.Application.Groups.Commands.RemoveGroupMember;

public record RemoveGroupMemberCommand(Guid GroupId, Guid UserId) : IRequest;
