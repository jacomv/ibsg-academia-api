using Academia.Application.Admin.Dtos;
using MediatR;

namespace Academia.Application.Users.Queries.GetOwnProfile;

public record GetOwnProfileQuery : IRequest<UserProfileDto>;
