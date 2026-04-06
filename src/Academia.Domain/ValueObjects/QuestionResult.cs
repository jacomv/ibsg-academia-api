namespace Academia.Domain.ValueObjects;

public record QuestionResult
{
    public Guid QuestionId { get; init; }
    public decimal PointsEarned { get; init; }
    public decimal MaxPoints { get; init; }
    public bool? IsCorrect { get; init; }
    public bool IsPending { get; init; }

    public static QuestionResult Correct(Guid questionId, decimal points) =>
        new() { QuestionId = questionId, PointsEarned = points, MaxPoints = points, IsCorrect = true, IsPending = false };

    public static QuestionResult Incorrect(Guid questionId, decimal maxPoints) =>
        new() { QuestionId = questionId, PointsEarned = 0, MaxPoints = maxPoints, IsCorrect = false, IsPending = false };

    public static QuestionResult Pending(Guid questionId, decimal maxPoints) =>
        new() { QuestionId = questionId, PointsEarned = 0, MaxPoints = maxPoints, IsCorrect = null, IsPending = true };
}
