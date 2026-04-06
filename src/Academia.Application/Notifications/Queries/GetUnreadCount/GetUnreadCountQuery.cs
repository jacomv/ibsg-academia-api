using MediatR;

namespace Academia.Application.Notifications.Queries.GetUnreadCount;

public record GetUnreadCountQuery : IRequest<int>;
