using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Groups.Commands.RemoveGroupMember;

public class RemoveGroupMemberCommandHandler : IRequestHandler<RemoveGroupMemberCommand>
{
    private readonly IApplicationDbContext _context;

    public RemoveGroupMemberCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(RemoveGroupMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _context.GroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == request.GroupId && m.UserId == request.UserId,
                cancellationToken);

        if (member is null)
            throw new NotFoundException("GroupMember", $"{request.GroupId}/{request.UserId}");

        _context.GroupMembers.Remove(member);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
