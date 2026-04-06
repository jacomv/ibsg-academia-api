using Academia.Domain.Common;
using Academia.Domain.Enums;

namespace Academia.Domain.Entities;

public class Grade : BaseEntity
{
    private Grade() { }

    public Grade(Guid userId, Guid examId, int attemptNumber,
        decimal totalScore, bool isPassed, GradingStatus status)
    {
        UserId = userId;
        ExamId = examId;
        AttemptNumber = attemptNumber;
        TotalScore = totalScore;
        IsPassed = isPassed;
        Status = status;
    }

    public Guid UserId { get; private set; }
    public Guid ExamId { get; private set; }
    public int AttemptNumber { get; private set; }
    public decimal TotalScore { get; private set; }
    public bool IsPassed { get; private set; }
    public GradingStatus Status { get; private set; }
    public string? TeacherFeedback { get; private set; }
    public Guid? GradedByTeacherId { get; private set; }
    public DateTime? GradedAt { get; private set; }

    public User User { get; private set; } = default!;
    public Exam Exam { get; private set; } = default!;
    public User? GradedByTeacher { get; private set; }

    public void ApplyManualGrade(decimal totalScore, bool isPassed, string? feedback, Guid teacherId)
    {
        TotalScore = totalScore;
        IsPassed = isPassed;
        TeacherFeedback = feedback;
        GradedByTeacherId = teacherId;
        GradedAt = DateTime.UtcNow;
        Status = GradingStatus.Manual;
    }
}
