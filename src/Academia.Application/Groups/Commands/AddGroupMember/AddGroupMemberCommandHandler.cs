using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Groups.Commands.AddGroupMember;

public class AddGroupMemberCommandHandler : IRequestHandler<AddGroupMemberCommand>
{
    private readonly IApplicationDbContext _context;

    public AddGroupMemberCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(AddGroupMemberCommand request, CancellationToken cancellationToken)
    {
        var groupExists = await _context.Groups
            .AnyAsync(g => g.Id == request.GroupId, cancellationToken);
        if (!groupExists)
            throw new NotFoundException("Group", request.GroupId);

        var userExists = await _context.Users
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
            throw new NotFoundException("User", request.UserId);

        var alreadyMember = await _context.GroupMembers
            .AnyAsync(m => m.GroupId == request.GroupId && m.UserId == request.UserId, cancellationToken);

        if (alreadyMember) return; // idempotent

        var member = new GroupMember(request.GroupId, request.UserId);
        await _context.GroupMembers.AddAsync(member, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
