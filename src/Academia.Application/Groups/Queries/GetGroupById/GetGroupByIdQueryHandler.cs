using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Groups.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Groups.Queries.GetGroupById;

public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, GroupDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetGroupByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<GroupDetailDto> Handle(
        GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups
            .Include(g => g.Members)
                .ThenInclude(m => m.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (group is null)
            throw new NotFoundException("Group", request.Id);

        return new GroupDetailDto(
            group.Id,
            group.Name,
            group.Description,
            group.Members
                .OrderBy(m => m.User.LastName)
                .Select(m => new GroupMemberDto(
                    m.UserId,
                    m.User.FullName,
                    m.User.Email,
                    m.JoinedAt))
                .ToList(),
            group.CreatedAt);
    }
}
