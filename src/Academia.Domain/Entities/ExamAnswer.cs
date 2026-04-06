using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class ExamAnswer : BaseEntity
{
    private ExamAnswer() { }

    public ExamAnswer(Guid userId, Guid examId, Guid questionId,
        string? answer, bool? isCorrect, decimal pointsEarned, int attemptNumber)
    {
        UserId = userId;
        ExamId = examId;
        QuestionId = questionId;
        Answer = answer;
        IsCorrect = isCorrect;
        PointsEarned = pointsEarned;
        AttemptNumber = attemptNumber;
    }

    public Guid UserId { get; private set; }
    public Guid ExamId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string? Answer { get; private set; }
    public bool? IsCorrect { get; private set; }
    public decimal PointsEarned { get; private set; }
    public int AttemptNumber { get; private set; }

    public User User { get; private set; } = default!;
    public Exam Exam { get; private set; } = default!;
    public Question Question { get; private set; } = default!;

    public void ApplyManualScore(decimal points, bool isCorrect)
    {
        PointsEarned = points;
        IsCorrect = isCorrect;
    }
}
