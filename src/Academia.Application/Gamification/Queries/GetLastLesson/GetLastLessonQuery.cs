using Academia.Application.Gamification.Dtos;
using MediatR;

namespace Academia.Application.Gamification.Queries.GetLastLesson;

public record GetLastLessonQuery : IRequest<LastLessonDto?>;
