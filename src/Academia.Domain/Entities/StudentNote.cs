using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class StudentNote : BaseEntity
{
    private StudentNote() { }

    public StudentNote(Guid userId, Guid lessonId, string content, int? timestampSeconds)
    {
        UserId = userId;
        LessonId = lessonId;
        Content = content;
        TimestampSeconds = timestampSeconds;
    }

    public Guid UserId { get; private set; }
    public Guid LessonId { get; private set; }
    public string Content { get; private set; } = default!;
    public int? TimestampSeconds { get; private set; }

    public User User { get; private set; } = default!;
    public Lesson Lesson { get; private set; } = default!;

    public void Update(string content, int? timestampSeconds)
    {
        Content = content;
        TimestampSeconds = timestampSeconds;
    }
}
