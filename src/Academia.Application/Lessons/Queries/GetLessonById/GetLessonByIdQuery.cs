using Academia.Application.Courses.Dtos;
using MediatR;

namespace Academia.Application.Lessons.Queries.GetLessonById;

public record GetLessonByIdQuery(Guid LessonId) : IRequest<LessonContentDto>;
