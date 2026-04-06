using Academia.Application.Courses.Dtos;
using MediatR;

namespace Academia.Application.Courses.Queries.GetCourseById;

public record GetCourseByIdQuery(Guid Id) : IRequest<CourseDetailDto>;
