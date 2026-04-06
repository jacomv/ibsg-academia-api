using Academia.Domain.Common;
using Academia.Domain.Enums;
using Academia.Domain.ValueObjects;

namespace Academia.Domain.Entities;

public class Question : BaseEntity
{
    private Question() { }

    public Question(Guid examId, QuestionType type, string text, List<string> options,
        string? correctAnswer, decimal points, int order)
    {
        ExamId = examId;
        Type = type;
        Text = text;
        Options = options;
        CorrectAnswer = correctAnswer;
        Points = points;
        Order = order;
    }

    public Guid ExamId { get; private set; }
    public QuestionType Type { get; private set; }
    public string Text { get; private set; } = default!;
    public List<string> Options { get; private set; } = new();
    public string? CorrectAnswer { get; private set; }
    public decimal Points { get; private set; }
    public int Order { get; private set; }

    public Exam Exam { get; private set; } = default!;

    public QuestionResult Evaluate(string? studentAnswer)
    {
        if (Type == QuestionType.Essay || Type == QuestionType.ShortAnswer)
            return QuestionResult.Pending(Id, Points);

        if (string.IsNullOrWhiteSpace(studentAnswer))
            return QuestionResult.Incorrect(Id, Points);

        var isCorrect = string.Equals(
            studentAnswer.Trim(),
            CorrectAnswer?.Trim(),
            StringComparison.OrdinalIgnoreCase);

        return isCorrect
            ? QuestionResult.Correct(Id, Points)
            : QuestionResult.Incorrect(Id, Points);
    }

    public void Update(QuestionType type, string text, List<string> options,
        string? correctAnswer, decimal points, int order)
    {
        Type = type;
        Text = text;
        Options = options;
        CorrectAnswer = correctAnswer;
        Points = points;
        Order = order;
    }
}
