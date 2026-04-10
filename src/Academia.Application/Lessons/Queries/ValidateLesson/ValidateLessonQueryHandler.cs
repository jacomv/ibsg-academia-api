using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Lessons.Validators;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Lessons.Queries.ValidateLesson;

public class ValidateLessonQueryHandler : IRequestHandler<ValidateLessonQuery, LessonValidationResult>
{
    private readonly IApplicationDbContext _context;

    public ValidateLessonQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<LessonValidationResult> Handle(
        ValidateLessonQuery request, CancellationToken cancellationToken)
    {
        var lesson = await _context.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.LessonId, cancellationToken);

        if (lesson is null)
            throw new NotFoundException("Lesson", request.LessonId);

        var result = LessonContentValidator.Validate(
            lesson.Type, lesson.TextContent, lesson.VideoUrl,
            lesson.AudioUrl, lesson.PdfFile);

        var errors = new List<string>(result.Errors);

        if (string.IsNullOrWhiteSpace(lesson.Title) || lesson.Title.Length < 3)
            errors.Add("Lesson title must be at least 3 characters.");

        return new LessonValidationResult(errors.Count == 0, errors);
    }
}
