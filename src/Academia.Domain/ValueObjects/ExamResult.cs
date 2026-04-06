using Academia.Domain.Enums;

namespace Academia.Domain.ValueObjects;

public record ExamResult
{
    public decimal TotalScore { get; init; }
    public bool IsPassed { get; init; }
    public GradingStatus GradingStatus { get; init; }
    public IReadOnlyList<QuestionResult> QuestionResults { get; init; } = [];
}
