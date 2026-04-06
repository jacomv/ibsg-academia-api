using FluentValidation;

namespace Academia.Application.Exams.Commands.SubmitExam;

public class SubmitExamCommandValidator : AbstractValidator<SubmitExamCommand>
{
    public SubmitExamCommandValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("ExamId is required.");

        RuleFor(x => x.Answers)
            .NotNull().WithMessage("Answers are required.")
            .Must(a => a.Count > 0).WithMessage("At least one answer is required.");

        RuleFor(x => x.StartedAt)
            .NotEmpty().WithMessage("StartedAt is required.")
            .LessThanOrEqualTo(_ => DateTime.UtcNow.AddMinutes(1))
            .WithMessage("StartedAt cannot be in the future.");
    }
}
