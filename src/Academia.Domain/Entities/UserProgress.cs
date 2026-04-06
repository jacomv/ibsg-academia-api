using Academia.Domain.Common;
using Academia.Domain.Enums;

namespace Academia.Domain.Entities;

public class UserProgress : BaseEntity
{
    private UserProgress() { }

    public UserProgress(Guid userId, Guid lessonId)
    {
        UserId = userId;
        LessonId = lessonId;
        Status = ProgressStatus.InProgress;
        ProgressPercentage = 0;
    }

    public Guid UserId { get; private set; }
    public Guid LessonId { get; private set; }
    public ProgressStatus Status { get; private set; }
    public int? VideoPosition { get; private set; }
    public int? AudioPosition { get; private set; }
    public decimal ProgressPercentage { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public User User { get; private set; } = default!;
    public Lesson Lesson { get; private set; } = default!;

    public void UpdatePosition(int? videoPosition, int? audioPosition, decimal progressPercentage)
    {
        if (Status == ProgressStatus.Completed) return;

        VideoPosition = videoPosition ?? VideoPosition;
        AudioPosition = audioPosition ?? AudioPosition;
        ProgressPercentage = progressPercentage;
        Status = ProgressStatus.InProgress;
    }

    public void MarkCompleted()
    {
        if (Status == ProgressStatus.Completed) return;
        Status = ProgressStatus.Completed;
        ProgressPercentage = 100;
        CompletedAt = DateTime.UtcNow;
    }
}
