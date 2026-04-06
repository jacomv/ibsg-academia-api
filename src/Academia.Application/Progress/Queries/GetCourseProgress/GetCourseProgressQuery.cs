using Academia.Application.Progress.Dtos;
using MediatR;

namespace Academia.Application.Progress.Queries.GetCourseProgress;

public record GetCourseProgressQuery(Guid CourseId) : IRequest<CourseProgressDto>;
