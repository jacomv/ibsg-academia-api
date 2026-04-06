using FluentValidation;

namespace Academia.Application.Courses.Commands.CreateCourse;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(300).WithMessage("Title must not exceed 300 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).When(x => x.Price.HasValue)
            .WithMessage("Price must be a positive value.");

        RuleFor(x => x.EstimatedDuration)
            .GreaterThan(0).When(x => x.EstimatedDuration.HasValue)
            .WithMessage("Estimated duration must be greater than 0.");
    }
}
