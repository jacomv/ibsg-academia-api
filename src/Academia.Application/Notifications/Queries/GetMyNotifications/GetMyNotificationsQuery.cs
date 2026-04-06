using Academia.Application.Notifications.Dtos;
using MediatR;

namespace Academia.Application.Notifications.Queries.GetMyNotifications;

public record GetMyNotificationsQuery(int Page = 1, int PageSize = 20) : IRequest<List<NotificationDto>>;
