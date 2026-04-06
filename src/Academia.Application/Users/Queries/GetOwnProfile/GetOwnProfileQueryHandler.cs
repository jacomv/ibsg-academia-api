using Academia.Application.Admin.Dtos;
using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Users.Queries.GetUserById;
using Academia.Domain.Interfaces;
using MediatR;

namespace Academia.Application.Users.Queries.GetOwnProfile;

public class GetOwnProfileQueryHandler : IRequestHandler<GetOwnProfileQuery, UserProfileDto>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public GetOwnProfileQueryHandler(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public async Task<UserProfileDto> Handle(GetOwnProfileQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedException();

        return await _mediator.Send(new GetUserByIdQuery(_currentUser.Id), cancellationToken);
    }
}
