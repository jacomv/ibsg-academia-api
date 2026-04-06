using Academia.Application.Common.Models;
using Academia.Application.Users.Dtos;
using Academia.Domain.Enums;
using MediatR;

namespace Academia.Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    int Page = 1,
    int PageSize = 20,
    UserRole? Role = null,
    string? Search = null,
    bool? IsActive = null
) : IRequest<PagedResult<UserListDto>>;
