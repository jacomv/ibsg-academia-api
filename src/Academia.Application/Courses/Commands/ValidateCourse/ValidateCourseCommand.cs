using MediatR;

namespace Academia.Application.Courses.Commands.ValidateCourse;

public record ValidateCourseCommand(Guid CourseId) : IRequest<CourseValidationResult>;

public record CourseValidationResult(
    bool IsValid,
    List<ValidationItem> Items
);

public record ValidationItem(
    string Rule,
    string Severity, // "Error" or "Warning"
    bool Passed,
    string? Message
);
