using Academia.Application.Groups.Dtos;
using MediatR;

namespace Academia.Application.Groups.Queries.GetGroups;

public record GetGroupsQuery : IRequest<List<GroupListDto>>;
