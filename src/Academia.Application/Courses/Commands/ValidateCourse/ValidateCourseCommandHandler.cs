using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Application.Lessons.Validators;
using Academia.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Courses.Commands.ValidateCourse;

public class ValidateCourseCommandHandler : IRequestHandler<ValidateCourseCommand, CourseValidationResult>
{
    private readonly IApplicationDbContext _context;

    public ValidateCourseCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<CourseValidationResult> Handle(
        ValidateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Chapters)
                .ThenInclude(ch => ch.Lessons)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course is null)
            throw new NotFoundException("Course", request.CourseId);

        var items = new List<ValidationItem>();

        // Rule 1: Title
        items.Add(new ValidationItem(
            "CourseTitle", "Error",
            !string.IsNullOrWhiteSpace(course.Title) && course.Title.Length >= 5,
            string.IsNullOrWhiteSpace(course.Title) ? "Course title is required."
                : course.Title.Length < 5 ? "Course title must be at least 5 characters." : null));

        // Rule 2: Description
        items.Add(new ValidationItem(
            "CourseDescription", "Error",
            !string.IsNullOrWhiteSpace(course.Description) && course.Description.Length >= 20,
            string.IsNullOrWhiteSpace(course.Description) ? "Course description is required."
                : course.Description.Length < 20 ? "Course description must be at least 20 characters." : null));

        // Rule 3: At least one chapter
        items.Add(new ValidationItem(
            "HasChapters", "Error",
            course.Chapters.Count > 0,
            course.Chapters.Count == 0 ? "Course must have at least one chapter." : null));

        // Rule 4: At least one playable lesson
        var playableLessons = course.Chapters
            .SelectMany(ch => ch.Lessons)
            .Where(l => l.Type != LessonType.Section)
            .ToList();

        items.Add(new ValidationItem(
            "HasLessons", "Error",
            playableLessons.Count > 0,
            playableLessons.Count == 0 ? "Course must have at least one lesson (non-section)." : null));

        // Rule 5: All lessons have valid content
        var lessonsWithErrors = new List<string>();
        foreach (var lesson in course.Chapters.SelectMany(ch => ch.Lessons))
        {
            var result = LessonContentValidator.Validate(
                lesson.Type, lesson.TextContent, lesson.VideoUrl,
                lesson.AudioUrl, lesson.PdfFile);
            if (!result.IsValid)
                lessonsWithErrors.Add($"'{lesson.Title}': {string.Join(", ", result.Errors)}");
        }

        items.Add(new ValidationItem(
            "LessonContent", "Error",
            lessonsWithErrors.Count == 0,
            lessonsWithErrors.Count > 0
                ? $"Lessons with missing content: {string.Join("; ", lessonsWithErrors)}" : null));

        // Rule 6: No duplicate order values within chapters
        var duplicateOrders = course.Chapters
            .Where(ch => ch.Lessons.GroupBy(l => l.Order).Any(g => g.Count() > 1))
            .Select(ch => ch.Title)
            .ToList();

        items.Add(new ValidationItem(
            "NoDuplicateOrder", "Warning",
            duplicateOrders.Count == 0,
            duplicateOrders.Count > 0
                ? $"Chapters with duplicate lesson orders: {string.Join(", ", duplicateOrders)}" : null));

        // Rule 7: Course image
        items.Add(new ValidationItem(
            "CourseImage", "Warning",
            !string.IsNullOrWhiteSpace(course.Image),
            string.IsNullOrWhiteSpace(course.Image) ? "Course image is recommended." : null));

        var hasErrors = items.Any(i => i.Severity == "Error" && !i.Passed);
        return new CourseValidationResult(!hasErrors, items);
    }
}
