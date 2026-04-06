using Academia.Domain.Common;
using Academia.Domain.Enums;
using Academia.Domain.ValueObjects;

namespace Academia.Domain.Entities;

public class Exam : BaseEntity
{
    private readonly List<Question> _questions = new();

    private Exam() { }

    public Exam(string title, Guid? courseId, Guid? chapterId,
        decimal passingScore, int maxAttempts, int? timeLimitMinutes, int order)
    {
        Title = title;
        CourseId = courseId;
        ChapterId = chapterId;
        PassingScore = passingScore;
        MaxAttempts = maxAttempts;
        TimeLimitMinutes = timeLimitMinutes;
        Order = order;
    }

    public string Title { get; private set; } = default!;
    public Guid? CourseId { get; private set; }
    public Guid? ChapterId { get; private set; }
    public decimal PassingScore { get; private set; }
    public int MaxAttempts { get; private set; }
    public int? TimeLimitMinutes { get; private set; }
    public int Order { get; private set; }

    public Course? Course { get; private set; }
    public Chapter? Chapter { get; private set; }
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    public bool CanAttempt(int attemptsMade) =>
        MaxAttempts == 0 || attemptsMade < MaxAttempts;

    public ExamResult Grade(IEnumerable<(Guid QuestionId, string? Answer)> submissions)
    {
        var results = _questions
            .Select(q => q.Evaluate(submissions.FirstOrDefault(s => s.QuestionId == q.Id).Answer))
            .ToList();

        var hasPending = results.Any(r => r.IsPending);
        var totalPoints = _questions.Sum(q => q.Points);
        var earnedPoints = results.Where(r => !r.IsPending).Sum(r => r.PointsEarned);
        var totalScore = totalPoints > 0 ? Math.Round(earnedPoints / totalPoints * 100, 2) : 0m;

        return new ExamResult
        {
            TotalScore = totalScore,
            IsPassed = !hasPending && totalScore >= PassingScore,
            GradingStatus = hasPending ? GradingStatus.Pending : GradingStatus.Auto,
            QuestionResults = results
        };
    }

    public void Update(string title, decimal passingScore, int maxAttempts,
        int? timeLimitMinutes, int order)
    {
        Title = title;
        PassingScore = passingScore;
        MaxAttempts = maxAttempts;
        TimeLimitMinutes = timeLimitMinutes;
        Order = order;
    }
}
