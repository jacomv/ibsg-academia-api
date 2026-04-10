using Academia.Domain.Common;

namespace Academia.Domain.Entities;

public class Bookmark : BaseEntity
{
    private Bookmark() { }

    public Bookmark(Guid userId, Guid lessonId)
    {
        UserId = userId;
        LessonId = lessonId;
    }

    public Guid UserId { get; private set; }
    public Guid LessonId { get; private set; }

    public User User { get; private set; } = default!;
    public Lesson Lesson { get; private set; } = default!;
}
