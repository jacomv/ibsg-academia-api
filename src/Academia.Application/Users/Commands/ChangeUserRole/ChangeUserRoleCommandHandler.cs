using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand>
{
    private readonly IApplicationDbContext _context;

    public ChangeUserRoleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null) throw new NotFoundException("User", request.UserId);

        user.ChangeRole(request.NewRole);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
