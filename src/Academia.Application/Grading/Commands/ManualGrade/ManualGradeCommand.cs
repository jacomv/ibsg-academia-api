using Academia.Application.Grading.Dtos;
using MediatR;

namespace Academia.Application.Grading.Commands.ManualGrade;

public record ManualGradeCommand(
    Guid GradeId,
    List<ManualScoreInput> Scores,
    string? Feedback
) : IRequest;
