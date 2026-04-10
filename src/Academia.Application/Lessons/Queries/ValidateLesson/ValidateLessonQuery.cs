using MediatR;

namespace Academia.Application.Lessons.Queries.ValidateLesson;

public record ValidateLessonQuery(Guid LessonId) : IRequest<LessonValidationResult>;

public record LessonValidationResult(bool IsValid, List<string> Errors);
