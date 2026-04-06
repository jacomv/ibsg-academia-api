using Academia.Application.Common.Interfaces;
using Academia.Application.Common.Models;
using Academia.Application.Users.Dtos;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<UserListDto>> Handle(
        GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsNoTracking().AsQueryable();

        if (request.Role.HasValue)
            query = query.Where(u => u.Role == request.Role.Value);

        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserListDto(
                u.Id, u.FirstName, u.LastName, u.Email, u.Role, u.IsActive, u.CreatedAt,
                u.Enrollments.Count(e => e.Status == EnrollmentStatus.Active)))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserListDto>(items, totalCount, request.Page, request.PageSize);
    }
}
