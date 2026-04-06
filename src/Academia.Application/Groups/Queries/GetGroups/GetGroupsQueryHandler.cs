using Academia.Application.Common.Interfaces;
using Academia.Application.Groups.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Groups.Queries.GetGroups;

public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, List<GroupListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetGroupsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<GroupListDto>> Handle(
        GetGroupsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Groups
            .Include(g => g.Members)
            .OrderBy(g => g.Name)
            .AsNoTracking()
            .Select(g => new GroupListDto(
                g.Id,
                g.Name,
                g.Description,
                g.Members.Count,
                g.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
