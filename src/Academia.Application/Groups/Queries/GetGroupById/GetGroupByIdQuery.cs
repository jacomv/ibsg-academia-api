using Academia.Application.Groups.Dtos;
using MediatR;

namespace Academia.Application.Groups.Queries.GetGroupById;

public record GetGroupByIdQuery(Guid Id) : IRequest<GroupDetailDto>;
