using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Users.Commands.UpdateOwnProfile;

public class UpdateOwnProfileCommandHandler : IRequestHandler<UpdateOwnProfileCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateOwnProfileCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateOwnProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.Id, cancellationToken);

        if (user is null) throw new NotFoundException("User", _currentUser.Id);

        user.UpdateProfile(request.FirstName, request.LastName, request.Avatar);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
