using MediatR;

namespace Academia.Application.Groups.Commands.EnrollGroup;

public record EnrollGroupCommand(Guid GroupId, Guid CourseId) : IRequest<int>;
